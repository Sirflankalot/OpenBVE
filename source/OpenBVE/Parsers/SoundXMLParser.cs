using System;
using System.IO;
using System.Xml;

namespace OpenBve
{
	class SoundXMLParser
	{
		//Parses an XML background definition
		public static Sounds.SoundBuffer ReadSoundXML(string fileName)
		{
			Sounds.SoundBuffer buffer = new Sounds.SoundBuffer();
			//The current XML file to load
			XmlDocument currentXML = new XmlDocument();
			//Load the object's XML file 
			currentXML.Load(fileName);
			string Path = System.IO.Path.GetDirectoryName(fileName);
			//Check for null
			if (currentXML.DocumentElement != null)
			{
				XmlNodeList DocumentNodes = currentXML.DocumentElement.SelectNodes("/openBVE/Sound");
				//Check this file actually contains a single sound node
				if (DocumentNodes != null && DocumentNodes.Count == 1)
				{
					foreach (XmlNode n in DocumentNodes)
					{
						if (n.HasChildNodes)
						{
							foreach (XmlNode c in n.ChildNodes)
							{
								switch (c.Name.ToLowerInvariant())
								{
									case "file":
										string b = OpenBveApi.Path.CombineFile(Path, c.InnerText);
										if (System.IO.File.Exists(b))
										{
											buffer.Origin = new Sounds.PathOrigin(b);
										}
										else
										{
											Interface.AddMessage(Interface.MessageType.Error, true, "The sound file " + b + " was not found in " + fileName);
											return null;
										}
										break;
									case "radius":
										if (!Double.TryParse(c.InnerText, out buffer.Radius))
										{
											Interface.AddMessage(Interface.MessageType.Error, true, "A value of " + c.InnerText + " is not a valid sound radius in " + fileName);
										}
										break;
									case "pitchfunction":
										try
										{
											buffer.PitchFunction = FunctionScripts.GetFunctionScriptFromInfixNotation(c.InnerText);
										}
										catch (Exception ex)
										{
											Interface.AddMessage(Interface.MessageType.Error, false, ex.Message + " in PitchFunction in file " + fileName);
										}
										break;
									case "volumefunction":
										try
										{
											buffer.VolumeFunction = FunctionScripts.GetFunctionScriptFromInfixNotation(c.InnerText);
										}
										catch (Exception ex)
										{
											Interface.AddMessage(Interface.MessageType.Error, false, ex.Message + " in PitchFunction in file " + fileName);
										}
										break;
									case "internalvolumefactor":
										if (!Double.TryParse(c.InnerText, out buffer.InternalVolumeFactor))
										{
											Interface.AddMessage(Interface.MessageType.Error, true, "A value of " + c.InnerText + " is not a valid volume factor in " + fileName);
										}
										break;
								}
							}

						}
					}
				}
				//Assuming our buffer's origin is not null, return it
				if (buffer.Origin != null)
				{
					return buffer;
				}
			}
			//We couldn't find any valid XML or the origin was not set, so return false
			throw new InvalidDataException();
		}
	}
}