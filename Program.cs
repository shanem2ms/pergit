using P4 = Perforce.P4;
using git = LibGit2Sharp;
using System.Diagnostics;
using LibGit2Sharp;
using Perforce.P4;


string subfolder_repo = "Vox";
// initialize the connection variables
// note: this is a connection without using a password
string uri = "shanem.ddns.net:1666";
string user = "shane";
string ws_client = "amdwork";


// define the server, repository and connection
P4.Server server = new P4.Server(new P4.ServerAddress(uri));
P4.Repository rep = new P4.Repository(server);
P4.Connection con = rep.Connection;


// use the connection variables for this connection
con.UserName = user;
con.Client = new P4.Client();
con.Client.Name = ws_client;

// connect to the server
bool didConnect = con.Connect(null);

string repodir = Path.Combine(con.Client.Root, subfolder_repo);
Console.WriteLine(repodir);

P4.ChangesCmdOptions options = new P4.ChangesCmdOptions(P4.ChangesCmdFlags.LongDescription | P4.ChangesCmdFlags.ReverseOrder,
        null, -1, P4.ChangeListStatus.None, null);

//git.Repository.Init(repodir);
git.Repository gitrepo = new git.Repository(repodir);

LocalPath localPath = new LocalPath(repodir + "\\...");
// run the command against the current repository
IList<P4.Changelist> changes = rep.GetChangelists(options, new P4.FileSpec(null, null, localPath, null));
bool first = true;
foreach (P4.Changelist changelist in changes)
{
    P4.SyncFilesCmdOptions syncOpts = new P4.SyncFilesCmdOptions(first ? P4.SyncFilesCmdFlags.Force : SyncFilesCmdFlags.None);
    P4.FileSpec depotFile = new P4.FileSpec(null, null, localPath, new P4.ChangelistIdVersion(changelist.Id));
    IList <P4.FileSpec> syncedFiles = rep.Connection.Client.SyncFiles(syncOpts, depotFile);
    Commands.Stage(gitrepo, "*");
    try
    {
        gitrepo.Commit("from script", new Signature("Shane Morrison", "shanemor@gmail.com", new DateTimeOffset(changelist.ModifiedDate)),
            new Signature("Shane Morrison", "shanemor@gmail.com", new DateTimeOffset(changelist.ModifiedDate)));
    }
    catch { }
    first = false;
    Console.WriteLine($"{changelist.Id} {changelist.ModifiedDate}");
}
