## Kee: simple and secure password management
Simple and secure password management. Login automatically, never forget another password, stay in control of your passwords and improve their security. Powered by the world-renowned KeePass Password Safe.

Kee (http://kee.pm) adds free, secure and easy to use password management features to Firefox which save you time and keep your private data more secure.

Kee connects Firefox to KeePass Password Safe, the most popular open source (and free) password manager.

## KeePassRPC plugin
This repository contains the source code for the KeePassRPC plugin that needs to be installed inside KeePass in order for Kee to be able to connect your browser to your passwords.

The official add-on can be downloaded at https://addons.mozilla.org/en-US/firefox/addon/keefox/

Please feel free to fork and submit pull requests for any changes or improvements you would like to see incorporated to the official add-on. See https://github.com/kee-org/KeeFox/wiki/en-|-Development for guidelines and more detail on how and what to develop.

Translate KeeFox into your language by starting here: https://github.com/kee-org/KeeFox/wiki/en-%7C-Contributing-%7C-Translation

Read about contributing to KeeFox in other ways here: https://github.com/kee-org/KeeFox/wiki/en-|-Contributing

## Troubleshooting
### Error during restart of KeePass 2.X after adding KeePassRPC.plgx to the plugins folder
KeePass error message: "The following plugin is incompatible with the current KeePass version: ..." If you are running KeePass 2.x under Mono, i.e. Linux, Mac OS X, BSD, etc., make sure mono-complete is installed. On Debian-based GNU/Linux distributions, installing the keepass2 package does NOT install mono-complete.
