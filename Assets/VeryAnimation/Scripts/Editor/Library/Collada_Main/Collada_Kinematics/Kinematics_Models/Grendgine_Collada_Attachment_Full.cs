using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
namespace VeryAnimation.grendgine_collada
{
	[System.SerializableAttribute()]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
	[System.Xml.Serialization.XmlRootAttribute(ElementName="attachment_full", Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=true)]
	public partial class Grendgine_Collada_Attachment_Full
	{
		[XmlAttribute("joint")]
		public string Joint;
		
		[XmlElement(ElementName = "translate")]
		public Grendgine_Collada_Translate[] Translate;

		[XmlElement(ElementName = "rotate")]
		public Grendgine_Collada_Rotate[] Rotate;		

		[XmlElement(ElementName = "link")]
		public Grendgine_Collada_Link Link;		
		
	}
}

