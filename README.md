********************************************************************************

BrewBot

This is a C# application that runs a Twitch bot which is primarily used for
playing drinking games with users in the chatroom. It supports other functionality
as well, such as posting preconfigured messages to chat, gambling in-chat mock
currency, running raffles, and storing configurations and login credentials.

********************************************************************************

Licensed under MIT

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

********************************************************************************

Configuration

 A BrewBot is configured using an XML document. It is recommended to make a copy
 of BrewBot\Config\config_default.xml to use as a configuration file. All configuration
 options must be nested inside of the <config></config> tags. The BrewBot
 is capable of sending a set of preconfigured messages to chat at regular intervals.
 You can configure the time interval between messages by setting the wait-time attribute
 of the interval tag. For example, <interval wait-time="60"/> will configure a one minute
 interval between messages. Messages can be configured by placing
 <message>your message here</message> tags inside of the <messages></messages> tags.
 
 You can also configure a default title for your subscribers. This will address
 subscribers to the stream in chat using the configured title. This is done using the
 <subscribers title="Your title"/> tag and attribute.
 
 You can configure channel currency and gambling using the <currency /> and <gambling />
 tags. Add the custom-name="Your currency name" attribute to the <currency /> tag to set
 the name of your channel's currency. Use the earn-rate="(integer earn rate)" attribute
 to set the rate at which currency is earned per minute while users are in the channel.
 If currency is configured, then gambling can also be configured. On the <gambling /> tag,
 use the attribute minimum="(integer minimum bet)" to set a minimum bet,
 gamble-interval="(seconds between bets)" to set a minimum wait time between bets, and
 odds="(decimal odds)" to set the probability of winning. The probability must be expressed
 as a decimal between 0 and 1 (inclusive)  <currency custom-name="cheddar" />
  <gambling odds="0.55"/>. For example:
 
 <currency custom-name="cheddar" earn-rate="200" />
 <gambling minimum="75" gamble-interval="30" odds="0.55"/>
 
 This configuration sets a currency named cheddar which is earned at a rate of 200
 per minute. A bet must be greater than or equal to 75 cheddar, can be executed up to
 once every thirty seconds, and has a 55% probability of winning.

********************************************************************************

Login

To login, you will need an authenticated Twitch.tv account. You can provide your Twitch
username, oauth token, chat channel, and an optional configuration file to login. It is
recommended to use a user other than your main account for the bot to avoid confusion.
You can get your Twitch oauth token here: http://twitchapps.com/tmi/
It is recommended to save your oauth token somewhere secure. When you put your oauth token
into the textbox it will be encrypted. Login by entering your username,
oauth, chat channel, and optionally selecting a configuration file. Then, if you wish your
login settings to be saved, check the Save Credentials checkbox. Then click login
to connect the bot to Twitch. Once your login information is saved, it can be retrieved
using the Load Credentials button. The login credentials are securely stored on your machine
in an xml file.

********************************************************************************

General Use

The bot will respond to commands in chat, send preconfigured messages, and monitor traffic
on it's own. Disconnect the bot from Twitch or exit the program using File->Disconnect
Exit the program by clicking File->Exit or by hitting the Alt + F4 keys. View the image window using
View->Image Window. You can navigate to the various other functions of the bot using the tabs
provided below File and View. Access the available commands for the bot by typing
!commands in chat. This will send the command list to chat up to once every two minutes to
avoid spamming the connection. Results of commands will be whispered to the user who issued
the command.

********************************************************************************

Raffle

The bot provides the ability to play raffles with your users to distribute prizes. Users can
enter a raffle by typing the !raffle command in chat. All raffle participants are shown in
the raffle tab. Users can be drawn from the raffle by clicking the Draw button. This will
highlight the username of the drawn user. Clear all raffle participants by clicking the clear button.

********************************************************************************

Drinking Game

Play a drinking game with your users using the Drinking Game tab. Check the
Play box to start playing the game. There are four available "Characters" representing
four available positions in the drinking game. Optionally, set the position's names by
changing the text in the appropriate text boxes. Players can then join the drinking
game by typing !join (position) in the chat. Viewers can be configured to have custom
positions by adding them using the Manual textbox/dropdown menu. Viewers in manual positions
can then be interacted with by selecting them from the Viewer textbox/dropdown menu. 

Notify all players to take drinks using the All Drink! button. Notify viewers in a particular
position to take a drink by using the Take a Drink button. Give viewers drink tickets by using
the Get a Drink Ticket button. Notify viewers to finish their drinks using the Finish Your Drink
button. A user can check how many drinks they have taken during drinking games using the !drinks
command. Note that finishing your drink only adds one to the running total.

A drink ticket represents the privilege of being able to share a drink with another drinking
game participant. A user can give a drink to someone by using the !give (username) command.
Users can check how many drink tickets they have to share by using the !tickets command.

A user can quit the drinking game by using the !quit command.

********************************************************************************

Gambling

If currency and gambling are configured, viewers will earn currency at a regular rate by
being present in the chat channel. Users can check their currency balance using the
!balance command. They can gamble currency using the !gamble (bet amount) command, provided
that the bet is greater than the minimum bet and less than or equal to the amount of
currency that the user has available.

Moderators in the chat can also "splash" all users with currency by using the
!splash (currency amount) command in chat.

********************************************************************************

Traffic

All traffic in the channel is monitored by the bot and can be viewed in the Traffic tab.
All messages have timestamps, and associated user names. Messages sent to the chat by
the bot are denoted with a < and messages sent to chat by other users are denoted with a >.
You can send messages to the chat using the textbox and the Send Message and Send Message Raw
buttons. Send Message Raw can be used to send custom format messages to the server and should
only be used if you know what you are doing.

For example, the below message is an outgoing message from the bot:
18:44:53.0 - chatbot < HeyGuys BrewBot ready for action!

The below message is an incoming message from chat:
18:53:41.9 - twitchuser23 > This is a message from another user in chat

********************************************************************************

Effects

This controls the View->Image Window view pane.

********************************************************************************