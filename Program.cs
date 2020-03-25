using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SerializeTest
{
    class Program
    {
        public static void Main(string[] args)
        {
            FacetContainer fileKindFacets = new FacetContainer();

            var facets = new List<FacetNode>()
            {
                new FacetNode
                {
                    Key = "pdf",
                    Count = 12,
                    RawValue = "pdf",
                },
                
                new FacetNode
                {
                    Key = "doc(x)",
                    Count = 10,
                    RawValue = "doc(x)",
                 }
            };
               
            FacetContainer facet01 = new FacetContainer(facets)
            {
                DisplayOrder = 1,
                IsTree = true,			
            };
            
            PopulateFileKindTree(facet01);

            var coreSerializer = System.Text.Json.JsonSerializer.Serialize(facet01);
            var newtonSerializer = Newtonsoft.Json.JsonConvert.SerializeObject(facet01);

            Console.WriteLine($"With System.Text.Json:\n{coreSerializer}\n");
            Console.WriteLine($"With Newtonsoft.Json:\n{newtonSerializer}");
            
            void PopulateFileKindTree(FacetContainer fileKindFacets)
            {
                var assetTypes = AssetTypes.GetAll();
                var roots = fileKindFacets.Facets.GroupJoin(
                    assetTypes,
                    f => f.Key,
                    a => a.ShortName,
                    (f, a) => new
                    {
                        Key = f.Key.ToString(),
                        Count = f.Count,
                        Value = a?.FirstOrDefault().Kind ?? AssetKind.Other
                    })
                    .GroupBy(a => a.Value)
                    .Select(g =>
                    {
                        var root = new FacetTreeNode
                        {
                            Key = g.Key.ToString(),
                            RawValue = g.Key.ToString(),
                            Leaf = false
                        };

                        var children = g.Select(a => new FacetTreeNode
                        {
                            Key = a.Key,
                            RawValue = a.Key,
                            Leaf = true,
                            Count = a.Count
                        });

                        root.Children.AddRange(children.OrderByDescending(c => c.Count));
                        root.Count = children.Sum(c => c.Count);
                        return root;
                    })
                    .OrderByDescending(r => r.Count)
                    .ToList();

                fileKindFacets.Facets.Clear();
                fileKindFacets.IsTree = true;
                fileKindFacets.Facets.AddRange(roots);
            }        
        }
    }

	public class FacetTreeNode : FacetNode
	{ 
        public FacetTreeNode()
		{	
			this.Children = new List<FacetTreeNode>();
		}
		
        [System.Text.Json.Serialization.JsonConverter(typeof(NodeSerializer<List<FacetTreeNode>>))]
		public List<FacetTreeNode> Children { get; set; }
		
		public bool Leaf { get; set; }
	}
	
	public class FacetNode
	{
	    public string Key { get; set; }

        public long Count { get; set; }

        public string RawValue { get; set; }

	}

	public class FacetContainer
	{
		public FacetContainer()
		{
			this.Facets = new List<FacetNode>();
		}		
		public FacetContainer(IEnumerable<FacetNode> facets)
			: this()
		{
			this.Facets.AddRange(facets);
		}
	
        public int DisplayOrder { get; set; }

        public bool IsTree { get; set; }

        public List<FacetNode> Facets { get; set; }
    }
												 
	public static class AssetTypes
	{
		static readonly IList<AssetType> all;
		
		static AssetTypes()
		{
			all = GetAllAssetTypes();
		}
		
		public static IList<AssetType> GetAll()
		{
			return all;
		}
		
		public static readonly AssetType Pdf = new AssetType("PDF", "pdf", AssetKind.Document);
		public static readonly AssetType Word = new AssetType("MS Word", "doc(x)", AssetKind.WordProcessor);
		
		private static IList<AssetType> GetAllAssetTypes()
        {
            var allFields = typeof(AssetTypes).GetFields(BindingFlags.Public | BindingFlags.Static);
            return allFields.Select(field => (AssetType)field.GetValue(null)).ToList();
        }	
	}
												 
	public class AssetType
    {
        public AssetType(string name, string shortName, AssetKind kind)
        {
            Name = name;
            ShortName = shortName;
            Kind = kind;
        }

        public string Name { get; }

        public string ShortName { get; }

        public AssetKind Kind { get; }

    }

    public enum AssetKind
    {
        Unknown,
        Other,
        WordProcessor,
        Document,
    }												 									 
}