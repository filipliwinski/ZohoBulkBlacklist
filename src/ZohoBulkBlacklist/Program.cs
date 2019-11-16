using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZohoBulkBlacklist
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Helo to Zoho Bulk Blacklist tool!");
            if (args.Length < 2 || args.Length > 4)
            {
                Console.WriteLine(@"Provide parameters:
[0]: path to the file containing the list of currently blocked domains (each domain in separate line)
[1]: path to the file containing the list of Zoho Mail quarantine (file name: example.com.txt)
[2]: path to the file containing the list of currently blocked domains from other tenants (optional)
[3]: number of new domains per row in the output (optional, default is 10, min. 10, max. 255)");
                Console.ReadKey();
            }

            // Add existing SPAM domains.
            var existingDomains = File.ReadAllLines(args[0]);
            var existingDomainsSet = new HashSet<string>(existingDomains.Length);

            for (int i = 0; i < existingDomains.Length; i++)
            {
                existingDomainsSet.Add(existingDomains[i].Trim());
            }

            File.WriteAllLines(args[0], existingDomainsSet);
            Console.WriteLine("Existing domains: " + existingDomainsSet.Count);

            var newDomains = new HashSet<string>();

            // Verify domains from other tenants.
            if (args.Length > 2)
            {
                var existingDomainsTenants = File.ReadAllLines(args[2]);
                for (int i = 0; i < existingDomainsTenants.Length; i++)
                {
                    var domain = existingDomainsTenants[i].Trim();
                    if (!existingDomainsSet.Contains(domain) && newDomains.Add(domain))
                    {
                        Console.WriteLine($"{newDomains.Count}: {domain}");
                    }
                }
            }
            else
            {
                Console.WriteLine("No other tenants provided.");
            }
            
            // Verify new SPAM addresses.
            var addresses = File.ReadAllLines(args[1]);

            for (int i = 0; i < addresses.Length; i++)
            {
                if (addresses[i].Contains("@") && 
                    !addresses[i].Contains("@" + args[1][0..^4]))   // Do not include tenant domain.
                {
                    var domain = addresses[i].Trim().Split('@')[1];
                    if (!existingDomainsSet.Contains(domain) && newDomains.Add(domain))
                    {
                        Console.WriteLine($"{newDomains.Count}: {domain}");
                    }
                }
            }

            // Save new SPAM domains.
            var output = new StringBuilder();
            if (newDomains.Count == 0)
            {
                output.Append("No new domains!");
                Console.WriteLine(output.ToString());
            }
            else
            {
                var j = 0;
                var k = 0;
                var domainsPerRowInput = 9;
                if (args.Length == 4)
                {
                    domainsPerRowInput = Convert.ToByte(args[3]) > 10 ? Convert.ToByte(args[3]) - 1 : 9;
                }
                var domainsPerRow = args.Length == 4 ? domainsPerRowInput : 9;
                output.Append($"[{k}] ");
                foreach (var item in newDomains)
                {
                    if (j++ != domainsPerRow)
                    {
                        output.Append(item).Append(',');
                    }
                    else
                    {
                        output.Append(item).Append(';').Append($"[{++k}] ");
                        j = 0;
                    }
                }
            }

            File.WriteAllLines("new" + args[0], output.ToString().TrimEnd(',').TrimEnd(';').Split(';'));
            Console.WriteLine("All done.");
        }
    }
}
