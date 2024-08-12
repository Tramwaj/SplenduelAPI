using Splenduel.Core.Home.Model;
using Splenduel.Core.Home.Store;
using System.IO;
using System.Security.Policy;

namespace TextStorage
{
    public class InviteStore(string filePath)
    {
        private string _invitesDir = filePath;

        public async Task<ICollection<GameInvite>> GetInvites(string player="")
        {
            var files = Directory.GetFiles(_invitesDir, "*.inv");
            var invites = new List<GameInvite>();
            foreach (var file in files)
            {
                string invData = await File.ReadAllTextAsync(file);
                var inv = invData.Split(',').Select(x => x.Trim()).ToArray();
                var invite = new GameInvite(Guid.Parse(inv[0]), DateTime.Parse(inv[1]), inv[2], inv[3], inv[4]);

                if (invite.Invitee.Contains(player) || invite.Inviter.Contains(player))
                    invites.Add(invite);
            }
            return invites;
        }
        public async Task<bool> SaveInvite(GameInvite invite)
        {            
            var pathToFile = $"{_invitesDir}/{invite.Id}.inv";
            if (File.Exists(pathToFile))
                return false;
            await File.AppendAllTextAsync(pathToFile,
                $"{invite.Id},{invite.TimeCreated},{invite.Inviter},{invite.Invitee},{invite.PlayerStarting}");
            return true;
        }

        public async Task<bool> RemoveInvite(Guid id)
        {
            var files = Directory.GetFiles(_invitesDir, "*.inv");
            var fileToDelete = files.FirstOrDefault(x => x.Contains(id.ToString()));
            if (string.IsNullOrEmpty(fileToDelete)) return false;
            File.Delete(fileToDelete);

            return true;
        }
    }
}
