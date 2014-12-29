Files
=====

File browser developed in C# with ASP.NET MVC for a custom website.

When the website is first loaded, it will ask you to create a password, which gets hashed upon saving, that you and other users can use to view the files.

This does not use a database because it just reads files from a folder and lists them out in a table. This allows people with access to the FTP of the website to upload files directly to the folder instead.
