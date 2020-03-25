# Serialize Tree Structure with Children

Using Newtonsoft.Json the FacetTreeNode children get serialized along with the rest of the tree structure. In contrast, using System.Text.Json, the FacetTreeNode children are not included. It appears that System.Text.Json doesn't serialize derived types automatically as explained in the [Serialize properties of derived classes](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to#serialize-properties-of-derived-classes).

I tried using a custom converter (NodeSerializer) as explained here: [System.Text.Json.JsonSerializer doesn't correctly serialize generic super class when nested](https://github.com/dotnet/runtime/issues/30425). See the Generic Workaround (write-only) post at the end of this thread. This didn't resolve the issue.
