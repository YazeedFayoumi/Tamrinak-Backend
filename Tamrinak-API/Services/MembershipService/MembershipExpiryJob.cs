namespace Tamrinak_API.Services.MembershipService
{
    public class MembershipExpiryJob : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;

        public MembershipExpiryJob(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(24)); // Runs daily
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using var scope = _scopeFactory.CreateScope();
            var membershipService = scope.ServiceProvider.GetRequiredService<IMembershipService>();

            var reminderCount = await membershipService.SendMembershipExpiryRemindersAsync();
            var deactivatedCount = await membershipService.DeactivateExpiredMembershipsAsync();

            Console.WriteLine($"[MembershipJob] {reminderCount} reminders sent, {deactivatedCount} memberships deactivated at {DateTime.UtcNow}");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();


    }
}
