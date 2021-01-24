//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder.Embedded v8.10.1
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.ModelsBuilder.Embedded;

namespace Umbraco.Web.PublishedModels
{
	/// <summary>Content Page</summary>
	[PublishedModel("contentPage")]
	public partial class ContentPage : PublishedContentModel, IJumbotron, INavigation, IUsefulLinks
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.10.1")]
		public new const string ModelTypeAlias = "contentPage";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.10.1")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.10.1")]
		public new static IPublishedContentType GetModelContentType()
			=> PublishedModelUtility.GetModelContentType(ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.10.1")]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<ContentPage, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(), selector);
#pragma warning restore 0109

		// ctor
		public ContentPage(IPublishedContent content)
			: base(content)
		{ }

		// properties

		///<summary>
		/// Content Grid: This is the Umbraco grid for layout
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.10.1")]
		[ImplementPropertyType("contentGrid")]
		public global::Newtonsoft.Json.Linq.JToken ContentGrid => this.Value<global::Newtonsoft.Json.Linq.JToken>("contentGrid");

		///<summary>
		/// Page Content: This is the page content
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.10.1")]
		[ImplementPropertyType("pageContent")]
		public global::System.Web.IHtmlString PageContent => this.Value<global::System.Web.IHtmlString>("pageContent");

		///<summary>
		/// HeroImage: This is the hero image displayed.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.10.1")]
		[ImplementPropertyType("heroImage")]
		public global::Umbraco.Core.Models.PublishedContent.IPublishedContent HeroImage => global::Umbraco.Web.PublishedModels.Jumbotron.GetHeroImage(this);

		///<summary>
		/// HeroSubTitle: This is the text that appears under the title.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.10.1")]
		[ImplementPropertyType("heroSubTitle")]
		public string HeroSubTitle => global::Umbraco.Web.PublishedModels.Jumbotron.GetHeroSubTitle(this);

		///<summary>
		/// HeroTitleText: This is the title text that appears on the hero image.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.10.1")]
		[ImplementPropertyType("heroTitleText")]
		public string HeroTitleText => global::Umbraco.Web.PublishedModels.Jumbotron.GetHeroTitleText(this);

		///<summary>
		/// Umbraco Navi Hide: If this is check then the page wont appear in the menu
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.10.1")]
		[ImplementPropertyType("umbracoNaviHide")]
		public bool UmbracoNaviHide => global::Umbraco.Web.PublishedModels.Navigation.GetUmbracoNaviHide(this);

		///<summary>
		/// Link Nested Content: This will be the nested content of links
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.10.1")]
		[ImplementPropertyType("linkNestedContent")]
		public global::System.Collections.Generic.IEnumerable<global::Umbraco.Web.PublishedModels.UsefulLink> LinkNestedContent => global::Umbraco.Web.PublishedModels.UsefulLinks.GetLinkNestedContent(this);
	}
}
