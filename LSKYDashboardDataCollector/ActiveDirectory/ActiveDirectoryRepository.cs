using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Hosting;
using Microsoft.SharePoint.Client;
using Principal = System.DirectoryServices.AccountManagement.Principal;

namespace LSKYDashboardDataCollector.ActiveDirectory
{
    public class ActiveDirectoryRepository
    {
        private readonly string domainName;

        public ActiveDirectoryRepository(string domain)
        {
            this.domainName = domain;
        }

        public List<string> GetGroupMemberUsernames(string groupName)
        {
            List<string> returnMe = new List<string>();
            using (HostingEnvironment.Impersonate())
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domainName))
                {
                    using (GroupPrincipal grp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, groupName))
                    {
                        if (grp != null)
                        {
                            foreach (Principal p in grp.GetMembers(true))
                            {
                                returnMe.Add(p.SamAccountName.ToLower());
                            }
                        }
                    }
                }
            }
            return returnMe;
        }

        public List<ADUser> GetGroupMembers(string groupName)
        {
            List<ADUser> returnMe = new List<ADUser>();

            foreach (string username in GetGroupMemberUsernames(groupName))
            {
                ADUser user = GetADUserFromUsername(username);
                if (user != null)
                {
                    returnMe.Add(user);
                }
            }

            return returnMe;
        }
        
        private ADUser GetADUserFromUsername(string username)
        {
            using (DirectoryEntry root = new DirectoryEntry())
            {
                using (DirectorySearcher searcher = new DirectorySearcher(root))
                {
                    searcher.Filter = "(sAMAccountName=" + username + ")";
                    searcher.PropertiesToLoad.Add("givenName");
                    searcher.PropertiesToLoad.Add("sn");
                    searcher.PropertiesToLoad.Add("sAMAccountName");
                    searcher.PropertiesToLoad.Add("comment");
                    searcher.PropertiesToLoad.Add("description");
                    searcher.PropertiesToLoad.Add("whenCreated");
                    searcher.PropertiesToLoad.Add("mail");
                    searcher.PropertiesToLoad.Add("userAccountControl");
                    searcher.PropertiesToLoad.Add("distinguishedName");
                    
                    SearchResult adSearchResult = searcher.FindOne();

                    if (adSearchResult != null)
                    {
                        // Get the directoryentry
                        DirectoryEntry user = adSearchResult.GetDirectoryEntry();

                        return new ADUser()
                        {
                            GivenName = (string)user.Properties["givenName"].Value,
                            SN = (string)user.Properties["sn"].Value,
                            sAMAccountName = (string)user.Properties["sAMAccountName"].Value,
                            comment = (string)user.Properties["comment"].Value,
                            description = (string)user.Properties["description"].Value,
                            IsEnabled = IsActive(user),
                            DateCreated = ((DateTime)user.Properties["whenCreated"].Value).ToString(),
                            DistinguishedName = (string)user.Properties["distinguishedName"].Value,
                            Mail = (string)user.Properties["mail"].Value
                        };

                    }
                }
            }

            return null;
        }
        
        private bool IsActive(DirectoryEntry de)
        {
            if (de.NativeGuid == null) return false;

            int flags = (int)de.Properties["userAccountControl"].Value;

            return !Convert.ToBoolean(flags & 0x0002);
        }

        public List<ADUser> GetAllDialInUsers()
        {
            List<ADUser> returnMe = new List<ADUser>();

            using (DirectoryEntry root = new DirectoryEntry())
            {
                using (DirectorySearcher searcher = new DirectorySearcher(root))
                {
                    searcher.Filter = "(msNPAllowDialin=TRUE)";
                    searcher.PropertiesToLoad.Add("givenName");
                    searcher.PropertiesToLoad.Add("sn");
                    searcher.PropertiesToLoad.Add("sAMAccountName");
                    searcher.PropertiesToLoad.Add("comment");
                    searcher.PropertiesToLoad.Add("description");
                    searcher.PropertiesToLoad.Add("whenCreated");
                    searcher.PropertiesToLoad.Add("mail");
                    searcher.PropertiesToLoad.Add("userAccountControl");
                    searcher.PropertiesToLoad.Add("distinguishedName");

                    SearchResultCollection adSearchResults = searcher.FindAll();

                    foreach (SearchResult adSearchResult in adSearchResults)
                    {
                        if (adSearchResult != null)
                        {
                            // Get the directoryentry
                            DirectoryEntry user = adSearchResult.GetDirectoryEntry();

                            returnMe.Add(new ADUser()
                            {
                                GivenName = (string) user.Properties["givenName"].Value,
                                SN = (string) user.Properties["sn"].Value,
                                sAMAccountName = (string) user.Properties["sAMAccountName"].Value,
                                comment = (string) user.Properties["comment"].Value,
                                description = (string) user.Properties["description"].Value,
                                IsEnabled = IsActive(user),
                                DateCreated = ((DateTime) user.Properties["whenCreated"].Value).ToString(),
                                DistinguishedName = (string) user.Properties["distinguishedName"].Value,
                                Mail = (string) user.Properties["mail"].Value
                            });

                        }
                    }
                }
            }
            return returnMe.OrderBy(u => u.sAMAccountName).ToList();
        } 

    }
}