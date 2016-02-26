using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIClient
{
    class Commits
    {
        public string sha { get; set; }
        public Object commit{ get; set; } /*"commit": {
    "author": {
      "name": "Ben Kyrlach",
      "email": "bkyrlach@racowireless.com",
      "date": "2016-02-24T13:53:46Z"
    },
    "committer": {
      "name": "Ben Kyrlach",
      "email": "bkyrlach@racowireless.com",
      "date": "2016-02-24T13:53:46Z"
    },*/
    public Object message { get; set; } /*": "Minor enhancements.",
    "tree": {
      "sha": "c04aa87f5da37579a602595c435f31a22b5ad153",
      "url": "https://api.github.com/repos/RacoWireless/FunctionalProgramming/git/trees/c04aa87f5da37579a602595c435f31a22b5ad153"
    },
    "url": "https://api.github.com/repos/RacoWireless/FunctionalProgramming/git/commits/3b1b516d02494c295d46012f14bd2818ad022f2b",
    "comment_count": 0
  },*/
  public string url { get; set; }
    public string html_url { get; set; }
    public string comments_url { get; set; }
    public Object author { get; set; }
    /*"login": "bkyrlach",
    "id": 1002529,
    "avatar_url": "https://avatars.githubusercontent.com/u/1002529?v=3",
    "gravatar_id": "",
    "url": "https://api.github.com/users/bkyrlach",
    "html_url": "https://github.com/bkyrlach",
    "followers_url": "https://api.github.com/users/bkyrlach/followers",
    "following_url": "https://api.github.com/users/bkyrlach/following{/other_user}",
    "gists_url": "https://api.github.com/users/bkyrlach/gists{/gist_id}",
    "starred_url": "https://api.github.com/users/bkyrlach/starred{/owner}{/repo}",
    "subscriptions_url": "https://api.github.com/users/bkyrlach/subscriptions",
    "organizations_url": "https://api.github.com/users/bkyrlach/orgs",
    "repos_url": "https://api.github.com/users/bkyrlach/repos",
    "events_url": "https://api.github.com/users/bkyrlach/events{/privacy}",
    "received_events_url": "https://api.github.com/users/bkyrlach/received_events",
    "type": "User",
    "site_admin": false
  },*/
  public Object committer { get; set; }
    /*"login": "bkyrlach",
    "id": 1002529,
    "avatar_url": "https://avatars.githubusercontent.com/u/1002529?v=3",
    "gravatar_id": "",
    "url": "https://api.github.com/users/bkyrlach",
    "html_url": "https://github.com/bkyrlach",
    "followers_url": "https://api.github.com/users/bkyrlach/followers",
    "following_url": "https://api.github.com/users/bkyrlach/following{/other_user}",
    "gists_url": "https://api.github.com/users/bkyrlach/gists{/gist_id}",
    "starred_url": "https://api.github.com/users/bkyrlach/starred{/owner}{/repo}",
    "subscriptions_url": "https://api.github.com/users/bkyrlach/subscriptions",
    "organizations_url": "https://api.github.com/users/bkyrlach/orgs",
    "repos_url": "https://api.github.com/users/bkyrlach/repos",
    "events_url": "https://api.github.com/users/bkyrlach/events{/privacy}",
    "received_events_url": "https://api.github.com/users/bkyrlach/received_events",
    "type": "User",
    "site_admin": false
  },*/
  public Object parents { get; set; }
    /*{
      "sha": "da483f23ab220b096add24723ecadbce4be0b1a9",
      "url": "https://api.github.com/repos/RacoWireless/FunctionalProgramming/commits/da483f23ab220b096add24723ecadbce4be0b1a9",
      "html_url": "https://github.com/RacoWireless/FunctionalProgramming/commit/da483f23ab220b096add24723ecadbce4be0b1a9"
    }
  ],*/
  public Object stats { get; set; }
  /*  "total": 5,
    "additions": 5,
    "deletions": 0
  },*/
  public Object files { get; set; }
    /*
    ": [
    {
      "sha": "ca5e53bac326a349af774a7cebad32118a9a22cb",
      "filename": "FunctionalProgramming/Helpers/FuncExtensions.cs",
      "status": "modified",
      "additions": 5,
      "deletions": 0,
      "changes": 5,
      "blob_url": "https://github.com/RacoWireless/FunctionalProgramming/blob/3b1b516d02494c295d46012f14bd2818ad022f2b/FunctionalProgramming/Helpers/FuncExtensions.cs",
      "raw_url": "https://github.com/RacoWireless/FunctionalProgramming/raw/3b1b516d02494c295d46012f14bd2818ad022f2b/FunctionalProgramming/Helpers/FuncExtensions.cs",
      "contents_url": "https://api.github.com/repos/RacoWireless/FunctionalProgramming/contents/FunctionalProgramming/Helpers/FuncExtensions.cs?ref=3b1b516d02494c295d46012f14bd2818ad022f2b",
      "patch": "@@ -9,6 +9,11 @@ public static class FuncExtensions\n             return f.Compose(m);\n         }\n \n+        public static Func<T1, T3> SelectMany<T1, T2, T3>(this Func<T1, T2> m, Func<T2, Func<T3>> f)\n+        {\n+            return x => f(m(x))();\n+        } \n+\n         public static Func<T2> Select<T1, T2>(this Func<T1> m, Func<T1, T2> f)\n         {\n             return () => f(m());"
    }*/
    }
}
