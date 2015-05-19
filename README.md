# Kfstorm.DoubanFM.Core
A .NET client library for douban.fm

**NOTE: This project is still under development.**

## Usage
1. Install [the nuget package](https://www.nuget.org/packages/Kfstorm.DoubanFM.Core/) to your project.
2. Create an instance of `IPlayer` interface and call the methods. Here is the sample code:

```
    var player = new Player(new Session(new ServerConnection("02646d3fb69a52ff072d47bf23cef8fd", "cde5d61429abcd7c", "radio_iphone", "100", new Uri("http://www.douban.com/mobile/fm"), Guid.NewGuid().ToString("N"))));
    await player.RefreshChannelList();
    var newChannel = player.ChannelList.ChannelGroups[0].Channels[0];
    var channelName = newChannel.Name;
    await player.ChangeChannel(newChannel);
    var currentSong = player.CurrentSong;
    var title = currentSong.Title;
    var url = currentSong.Url;
    await player.Next(NextCommandType.SkipCurrentSong);
```
