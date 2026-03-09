using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Xml;
using System.Xml.Serialization;

namespace Application.Queries;

public class GibUserService
{
    private readonly GibUserOptions _options;
    private readonly IGibUserRepository _repository;

    public GibUserService(IOptions<GibUserOptions> options,
        IGibUserRepository repository)
    {
        _options = options.Value;
        _repository = repository;
    }

    public async Task UpdateGibUserList()
    {
        await UpdateGibUserList(_options.GbListUrl, Unit.GB);
        await UpdateGibUserList(_options.PkListUrl, Unit.PK);
    }

    private async Task UpdateGibUserList(string url, Unit unit)
    {
        string zipFile = Path.GetFileName(url);
        FileInfo? file = null;
        if (File.Exists(zipFile)) file = new(zipFile);
        if (file?.CreationTime == null || file.CreationTime <= DateTime.Now.AddHours(-2))
        {
            await Download(url);
            using ZipArchive archive = ZipFile.OpenRead(zipFile);
            string xmlFile = archive.Entries[0].Name;
            ZipFile.ExtractToDirectory(zipFile, "./", true);
            UserList users = Deserialize(xmlFile);
            await Delete(unit);
            await Insert(users, unit);
        }
    }

    private static async Task Download(string url)
    {
        using HttpClient client = new();
        using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        using Stream remoteStream = await response.Content.ReadAsStreamAsync();
        string fileName = Path.GetFileName(url);
        using FileStream localStream = new(fileName, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
        await remoteStream.CopyToAsync(localStream);
    }

    private static UserList Deserialize(string path)
    {
        using FileStream stream = new(path, FileMode.Open, FileAccess.Read);
        using XmlReader reader = XmlReader.Create(stream);
        XmlSerializer xs = new(typeof(UserList));
        UserList userList = (UserList?)xs.Deserialize(reader)
            ?? throw new Exception("İçerik deserialize edilemedi.");
        return userList;
    }

    private async Task Delete(Unit unit)
    {
        List<GibUser> users = await _repository.GetGibUser(unit);
        _repository.Remove(users);
        await _repository.Save();
    }

    private async Task Insert(UserList userList, Unit unit)
    {
        ConcurrentBag<GibUser> gibUsers = [];
        await Parallel.ForEachAsync(userList.User,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            (user, ct) =>
            {
                foreach (var doc in user.Documents)
                {
                    if (doc.Alias == null) continue;
                    foreach (AliasType alias in doc.Alias)
                    {
                        foreach (string name in alias.Name)
                        {
                            GibUser gibUser = new()
                            {
                                Identifier = user.Identifier,
                                Title = user.Title,
                                UserType = user.Type,
                                AccountType = user.AccountType,
                                FirstCreationTime = user.FirstCreationTime,
                                DocumentType = doc.type,
                                Unit = unit,
                                Alias = name,
                                AliasCreationTime = alias.CreationTime,
                                AliasDeletionTime = alias.DeletionTime,
                                IsActive = alias.DeletionTime == null
                            };
                            gibUsers.Add(gibUser);
                        }
                    }
                }
                return ValueTask.CompletedTask;
            });
        await _repository.Add(gibUsers);
        await _repository.Save();
    }
}