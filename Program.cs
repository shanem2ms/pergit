using P4 = Perforce.P4;
using git = LibGit2Sharp;
using System.Diagnostics;

// initialize the connection variables
// note: this is a connection without using a password
string uri = "shanem.ddns.net:1666";
string user = "shane";
string ws_client = "gitmidiplayer";


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

Console.WriteLine(con.Client.Root);

P4.ChangesCmdOptions options = new P4.ChangesCmdOptions(P4.ChangesCmdFlags.LongDescription,
        null, -1, P4.ChangeListStatus.None, null);

git.Repository gitrepo = new git.Repository(@"C:\midiplayer");
foreach (var branch in gitrepo.Branches)
{
    Debug.WriteLine(branch);
}

// run the command against the current repository
IList<P4.Changelist> changes = rep.GetChangelists(options, null);
foreach (P4.Changelist changelist in changes)
{
    P4.SyncFilesCmdOptions syncOpts = new P4.SyncFilesCmdOptions(P4.SyncFilesCmdFlags.Force);
    P4.FileSpec depotFile = new P4.FileSpec(new P4.DepotPath("//depot/midiplayer/..."), null, null, new P4.ChangelistIdVersion(changelist.Id));
    IList <P4.FileSpec> syncedFiles = rep.Connection.Client.SyncFiles(syncOpts, depotFile);
    Console.WriteLine($"{changelist.Id} {changelist.ModifiedDate}");    
    break;
}
