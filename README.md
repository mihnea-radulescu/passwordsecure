# passwordsecure
Password Secure is a cross-platform, offline password manager, using strong AES-256 encryption.

It is written in C#, and targets .NET 8 on Linux and Windows. It relies on [Avalonia](https://github.com/AvaloniaUI/Avalonia), as its UI framework.

Password Secure encrypts the user's accounts and web sites login data, using a master password.

Features:
* state-of-the-art AES-256 symmetric-key encryption
* offline storage, using portable files
* automatic backup creation, before updating the stored data
* secure copying and editing of passwords, without revealing them
* automatic clipboard clearing on application exit

![Screenshot 1](https://raw.githubusercontent.com/mihnea-radulescu/passwordsecure/main/Screenshot-Dark.jpg "Password Secure - Dark Screenshot")

![Screenshot 2](https://raw.githubusercontent.com/mihnea-radulescu/passwordsecure/main/Screenshot-Light.jpg "Password Secure - Light Screenshot")
