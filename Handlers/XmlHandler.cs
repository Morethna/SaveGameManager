using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Windows;
using System.Xml;
using SaveGameManager.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SaveGameManager.Handler
{
    public class XmlHandler
  {
    const string _profileXml = "profile.xml";
    const string _idAtt = "Id";
    const string _nameAtt = "Name";


    const string _creationTimeAtt = "CreationTime";
    const string _gameFolderNode = "Gamefolder";
    const string _rootNode = "SaveGameManager";
    const string _profileNode = "Profile";
    const string _profilesNode = "Profiles";

    private List<Profile> _profiles = new List<Profile>();
    private string _gamefolder = string.Empty;
    private XmlDocument _document = new XmlDocument();
    private string _xmlPath = string.Empty;

    public XmlHandler(List<Profile> profiles) 
    {
      _profiles = profiles;
    }

    public List<Profile> Profiles { get => _profiles; } 
    public string GameFolder { get => _gamefolder; }

    public void Init()
    {
      _xmlPath = Path.Combine(Directory.GetCurrentDirectory(), _profileXml);

      if (!File.Exists(_xmlPath))
        CreateXml();
      else
        LoadXml();
    }

    public void SetGamefolder (string gamePath)
    {
      try
      {
        var node = _document.SelectSingleNode($"*/{_gameFolderNode}");
        node.InnerText = gamePath;
        _document.Save(_xmlPath);
        _gamefolder = gamePath;
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Something went wrong, while setting gamefolder '{gamePath}'.\r\n{ex.Message}");
      }
    }

    public void AddProfile(Profile profile)
    {
      try
      {
        var node = _document.SelectSingleNode($"*/{_profilesNode}");
        var nProfile = _document.CreateElement(_profileNode);

        nProfile.SetAttribute(_idAtt, profile.Id);
        nProfile.SetAttribute(_nameAtt, profile.Name);
        nProfile.SetAttribute(_creationTimeAtt, profile.CreationTime);

        node.AppendChild(nProfile);
        _document.Save(_xmlPath);
        _profiles.Add(profile);
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Something went wrong, while adding profile '{profile.Name}'.\r\n{ex.Message}");
      }
    }

    public void EditProfile(Profile profile)
    {
      try
      {
        var node = _document.SelectSingleNode($"*/{_profilesNode}/{_profileNode}[@Id='{profile.Id}']");

        if (node != null)
        {
          node.Attributes[_nameAtt].Value = profile.Name;
        }
        _document.Save(_xmlPath);
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Something went wrong, while editing profile '{profile.Name}'.\r\n{ex.Message}");
      }
    }

    public void DeleteProfile(Profile profile)
    {
      try
      {
        var node = _document.SelectSingleNode($"*/{_profilesNode}/{_profileNode}[@Id='{profile.Id}']");
        
        if (node != null)
        {
          var parent = _document.SelectSingleNode($"*/{_profilesNode}");
          parent.RemoveChild(node);
        }
        _document.Save(_xmlPath);
        _profiles.Remove(profile);
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Something went wrong, while editing profile '{profile.Name}'.\r\n{ex.Message}");
      }
    }

    private void CreateXml()
    {
      try
      {
        _document.AppendChild(_document.CreateXmlDeclaration("1.0", "UTF-8", null));

        var rootNode = _document.CreateElement(_rootNode);
        rootNode.AppendChild(_document.CreateElement(_gameFolderNode));
        rootNode.AppendChild(_document.CreateElement(_profilesNode));

        _document.AppendChild(rootNode);
        _document.Save(_xmlPath);
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Something went wrong, while creating Xml '{_xmlPath}'.\r\n{ex.Message}");
      }
    }

    private void LoadXml()
    {
      try
      {
        _document.Load(_xmlPath);
        _gamefolder = _document.SelectSingleNode("*/Gamefolder").InnerText;

        foreach (XmlNode node in _document.SelectSingleNode($"*/{_profilesNode}").ChildNodes)
        {
          _profiles.Add(new Profile
          {
            Name = node.Attributes[_nameAtt].Value,
            CreationTime = node.Attributes[_creationTimeAtt].Value,
            Id = node.Attributes[_idAtt].Value
          });
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Something went wrong, while loading Xml '{_xmlPath}'.\r\n{ex.Message}");
      }
    }
  }
}
