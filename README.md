# Lithnet FIM/MIM Linux/Unix SSH Management Agent
The Lithnet SSH MA is a ForeFront Identity Manager (FIM) ECMA2.2 management agent used to provision and synchronize objects to unix and linux systems using SSH

The management agent supports
* Full and (optionally) delta imports
* Exports (supporting either object replace, attribute replace, attribute update, or multivalued reference attribute update modes)
* Password set and change
* Username and RSA key-based logins, as well as username/password logins
* Dynamic DN construction

*This MA requires ECMA2.2 which is supported in FIM 4.1.3441.0 and above*

This management agent utilizes the RENCI SSH.NET library http://sshnet.codeplex.com/
