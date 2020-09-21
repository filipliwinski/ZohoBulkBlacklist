# ZohoBulkBlacklist

Simple app for bulk adding SPAM sending domains to the Zoho Mail blocked list.

Read more: [Organization Spam Control](https://www.zoho.com/mail/help/adminconsole/organization-spam-control.html) > [Blacklist Email/Domains](https://www.zoho.com/mail/help/adminconsole/organization-spam-control.html#alink8)

#### App parameters

[0]: the path to the file containing the list of currently blocked domains (each domain in a separate line)\
[1]: the path to the file containing the list from the Zoho Mail quarantine (file name: example.com.txt)\
[2]: the path to the file containing the list of currently blocked domains from other tenants (optional)\
[3]: number of new domains per row in the output (optional, default is 10, min. 10, max. 255)
