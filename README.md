# Trinity
Trinity is a sort of layer that sits between your bot's codebase and a wrapper for a given platform.  
It aims to enable you to easily use common features implemented by trinity wrappers for wrappers for a given platform but if something is not implemented in a given trinity wrapper you can easily cast to a given trinity wrapper from the trinity interfaces to access the platform wrapper's functionalty directly.  
Current support is in Proof Of Concept state and will be treated as such.  
Basic methods such as ITrinityChannel.SendMessageAsync(string message) should be implemented.  
