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
    const string _activeProfileNode = "ActiveProfile";
    const string _rootNode = "SaveGameManager";
    const string _profileNode = "Profile";
    const string _profilesNode = "Profiles";

    private List<Profile> _profiles = new List<Profile>();
    private string _gamefolder = string.Empty;
    private string _activeProfile = string.Empty;
    private XmlDocument _document = new XmlDocument();
    private string _xmlFilePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/SaveGameManager/{_profileXml}";
    private string _xmlPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/SaveGameManager";


    public XmlHandler(List<Profile> profiles) 
    {
      _profiles = profiles;
    }

    public List<Profile> Profiles { get => _profiles; } 
    public string GameFolder { get => _gamefolder; }
    public string ActiveProfile { get => _activeProfile; }
    public void Init()
    {
      if (!Directory.Exists(_xmlPath))
        Directory.CreateDirectory(_xmlPath);

      if (!File.Exists(_xmlFilePath))
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
        _document.Save(_xmlFilePath);
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
        var profileNode = _document.SelectSingleNode($"*/{_profilesNode}");
        var nProfile = _document.CreateElement(_profileNode);
        var activeProfile = _document.SelectSingleNode($"*/{_activeProfileNode}");
        var parent = _document.SelectSingleNode(_rootNode);

        nProfile.SetAttribute(_idAtt, profile.Id);
        nProfile.SetAttribute(_nameAtt, profile.Name);
        nProfile.SetAttribute(_creationTimeAtt, profile.CreationTime);

        profileNode.AppendChild(nProfile);

        if (activeProfile != null)
          parent.RemoveChild(activeProfile);

        var node = _document.CreateElement(_activeProfileNode); 
        node.InnerText = profile.Id;
        parent.AppendChild(node);

        _document.Save(_xmlFilePath);
        _profiles.Add(profile);
        _activeProfile = profile.Id;
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
        _document.Save(_xmlFilePath);
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Something went wrong, while editing profile '{profile.Name}'.\r\n{ex.Message}");
      }
    }

    public void ChangeProfile(string profileId)
    {
      try
      {
        var node = _document.SelectSingleNode($"*/{_activeProfileNode}");
        var parent = _document.SelectSingleNode(_rootNode);

        if (node != null)
          parent.RemoveChild(node);

        node = _document.CreateElement(_activeProfileNode);
        node.InnerText = profileId;
        parent.AppendChild(node);

        _document.Save(_xmlFilePath);
        _activeProfile = profileId;
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Something went wrong, while changing the active profile '{profileId}'.\r\n{ex.Message}");
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
        _profiles.Remove(profile);

        if (profile.Id == _activeProfile)
        {
          node = _document.SelectSingleNode($"*/{_activeProfileNode}");

          if (node != null && node.InnerText == _activeProfile)
          {
            var parent = _document.SelectSingleNode(_rootNode);
            parent.RemoveChild(node);
          }
          _activeProfile = string.Empty;
        }

        _document.Save(_xmlFilePath);       
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
        rootNode.AppendChild(_document.CreateElement(_activeProfileNode));
        rootNode.AppendChild(_document.CreateElement(_profilesNode));

        _document.AppendChild(rootNode);
        _document.Save(_xmlFilePath);
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Something went wrong, while creating Xml '{_xmlFilePath}'.\r\n{ex.Message}");
      }
    }

    private void LoadXml()
    {
      try
      {
        _document.Load(_xmlFilePath);

        var nodeBase = _document.SelectSingleNode($"*/{_gameFolderNode}");

        if (nodeBase == null)
          throw new Exception($"\"{_gameFolderNode}\" is missing in the \"{_xmlFilePath}\" file. Add this Node manually or delete this file.");

        _gamefolder = nodeBase.InnerText;

        nodeBase = _document.SelectSingleNode($"*/{_activeProfileNode}");

        if (nodeBase != null)
          _activeProfile = nodeBase.InnerText;

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
        MessageBox.Show($"Something went wrong, while loading Xml '{_xmlFilePath}'.\r\n{ex.Message}");
      }
    }
  }
}
