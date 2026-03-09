using Application.Queries;
using Quartz;

namespace Worker
{
    internal class GibUserUpdaterJob : IJob
    {
        private readonly GibUserService _gibUser;

        public GibUserUpdaterJob(GibUserService gibUser)
        {
            _gibUser = gibUser;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _gibUser.UpdateGibUserList();
        }
    }
}
