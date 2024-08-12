using NUnit.Framework.Internal;
using Splenduel.Core.Home.Model;

namespace TextStorage.Tests
{
    public class InviteStoreTests
    {
        private InviteStore _inviteStore;
        private string invitesDirectory = $"{TestContext.CurrentContext.TestDirectory}/Invites";
        private readonly DateTime[] dates = { DateTime.Today, DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-2) };
        private readonly string[] users = { "user1", "user2", "user3", "user4" };
        private readonly Guid[] guids = { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        [SetUp]
        public void Setup()
        {
            _inviteStore = new(invitesDirectory);
            Directory.CreateDirectory(invitesDirectory);
        }
        //[Test]
        public async Task InviteStoreReturnsTrueWhenAddingInvite()
        {
            var added = new GameInvite(guids[0], dates[0], users[0], users[1], users[0]);
            var response = await _inviteStore.SaveInvite(added);
            Assert.IsTrue(response);
        }


        //[Test]
        public async Task InviteStore_Should_Store_Invite_Single()
        {
            var inviter = users[3];
            var invitee = users[2];
            var playerStarting = users[3];
            var added = new GameInvite(guids[1],dates[1], inviter, invitee, playerStarting);
            var response = await _inviteStore.SaveInvite(added);
            var retrieved = await _inviteStore.GetInvites();
            Console.Write(retrieved.ToString());

            Assert.IsNotNull(retrieved);
            Assert.That(retrieved.Count, Is.EqualTo(1));
            Assert.That(added.Id, Is.EqualTo(guids[1]));
            Assert.That(added.Inviter, Is.EqualTo(inviter));
            Assert.That(added.Invitee, Is.EqualTo(invitee));
            Assert.That(added.PlayerStarting, Is.EqualTo(playerStarting));
            Assert.That(added.TimeCreated, Is.EqualTo(dates[1]));
        }
        //[Test]
        public async Task Removing_Invites_Should_Return_False_When_Empty()
        {
            var response = await _inviteStore.RemoveInvite(Guid.NewGuid());
            Assert.IsFalse(response);
        }
        //[Test]
        public async Task Removing_Wrong_Invites_Should_Not_Remove_Any()
        {
            var inviter = users[3];
            var invitee = users[2];
            var playerStarting = users[3];
            var added = new GameInvite(guids[1], dates[1], inviter, invitee, playerStarting);
            await _inviteStore.SaveInvite(added);
            bool response = await _inviteStore.RemoveInvite(Guid.NewGuid());
            var invites = await _inviteStore.GetInvites();
            Assert.That(response,Is.False);
            Assert.That(invites.Count(), Is.EqualTo(1));
        }
        [TearDown]
        public void TearDown()
        {
            var files = Directory.GetFiles(invitesDirectory);
            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
        }
    }
}