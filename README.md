# Trinity
Trinity is a sort of layer that sits between your bot's codebase and a wrapper for a given platform.  
It aims to enable you to easily use common features implemented by trinity wrappers for wrappers for a given platform but if something is not implemented in a given trinity wrapper you can easily cast to a given trinity wrapper from the trinity interfaces to access the platform wrapper's functionalty directly.  
Current support is in Proof Of Concept state and will be treated as such.  
Basic methods such as ITrinityChannel.SendMessageAsync(string message) should be implemented.  
DSharpPlus.CommandsNext (https://github.com/DSharpPlus/DSharpPlus/tree/master/DSharpPlus.CommandsNext) has been ported to trinity in a basic way.  

By signing off to a commit of this repository you agree to the terms of the Developer Certificate of Origin which can be found at https://developercertificate.org and reads 
```
Developer Certificate of Origin
Version 1.1

Copyright (C) 2004, 2006 The Linux Foundation and its contributors.

Everyone is permitted to copy and distribute verbatim copies of this
license document, but changing it is not allowed.


Developer's Certificate of Origin 1.1

By making a contribution to this project, I certify that:

(a) The contribution was created in whole or in part by me and I
    have the right to submit it under the open source license
    indicated in the file; or

(b) The contribution is based upon previous work that, to the best
    of my knowledge, is covered under an appropriate open source
    license and I have the right under that license to submit that
    work with modifications, whether created in whole or in part
    by me, under the same open source license (unless I am
    permitted to submit under a different license), as indicated
    in the file; or

(c) The contribution was provided directly to me by some other
    person who certified (a), (b) or (c) and I have not modified
    it.

(d) I understand and agree that this project and the contribution
    are public and that a record of the contribution (including all
    personal information I submit with it, including my sign-off) is
    maintained indefinitely and may be redistributed consistent with
    this project or the open source license(s) involved.
```
