﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BrewBot.Commands.Factory {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class CommandResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CommandResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BrewBot.Commands.Factory.CommandResources", typeof(CommandResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to !gamble (amount) - Gamble currency.
        /// </summary>
        internal static string Casino_Gamble_Description {
            get {
                return ResourceManager.GetString("Casino_Gamble_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ^!gamble (.*)$.
        /// </summary>
        internal static string Casino_Gamble_Regex {
            get {
                return ResourceManager.GetString("Casino_Gamble_Regex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &quot;!balance - Display your currency balance&quot;.
        /// </summary>
        internal static string Casino_GetBalance_Description {
            get {
                return ResourceManager.GetString("Casino_GetBalance_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ^!balance$.
        /// </summary>
        internal static string Casino_GetBalance_Regex {
            get {
                return ResourceManager.GetString("Casino_GetBalance_Regex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to !splash (currency amount) - Moderator Only : Give ALL active users the desired amount of currency.
        /// </summary>
        internal static string Casino_Splash_Description {
            get {
                return ResourceManager.GetString("Casino_Splash_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ^!splash (.*)$.
        /// </summary>
        internal static string Casino_Splash_Regex {
            get {
                return ResourceManager.GetString("Casino_Splash_Regex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to !drinks - Display how many drinks you have taken during active drinking games.
        /// </summary>
        internal static string DrinkingGame_GetDrinks_Description {
            get {
                return ResourceManager.GetString("DrinkingGame_GetDrinks_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ^!drinks$.
        /// </summary>
        internal static string DrinkingGame_GetDrinks_Regex {
            get {
                return ResourceManager.GetString("DrinkingGame_GetDrinks_Regex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to !tickets - Display your drink ticket balance.
        /// </summary>
        internal static string DrinkingGame_GetTickets_Description {
            get {
                return ResourceManager.GetString("DrinkingGame_GetTickets_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ^!tickets$.
        /// </summary>
        internal static string DrinkingGame_GetTickets_Regex {
            get {
                return ResourceManager.GetString("DrinkingGame_GetTickets_Regex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to !give (username) - Spend a drink ticket to make a player drink.
        /// </summary>
        internal static string DrinkingGame_GiveDrink_Description {
            get {
                return ResourceManager.GetString("DrinkingGame_GiveDrink_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ^!give (.*)$.
        /// </summary>
        internal static string DrinkingGame_GiveDrink_Regex {
            get {
                return ResourceManager.GetString("DrinkingGame_GiveDrink_Regex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to !join (player) - Join the drinking game as the input player.
        /// </summary>
        internal static string DrinkingGame_Join_Description {
            get {
                return ResourceManager.GetString("DrinkingGame_Join_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ^!join (.*)$.
        /// </summary>
        internal static string DrinkingGame_Join_Regex {
            get {
                return ResourceManager.GetString("DrinkingGame_Join_Regex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to !quit - Quit the drinking game.
        /// </summary>
        internal static string DrinkingGame_Quit_Description {
            get {
                return ResourceManager.GetString("DrinkingGame_Quit_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ^!quit$.
        /// </summary>
        internal static string DrinkingGame_Quit_Regex {
            get {
                return ResourceManager.GetString("DrinkingGame_Quit_Regex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to !commands - get this list of commands.
        /// </summary>
        internal static string GetCommands_Description {
            get {
                return ResourceManager.GetString("GetCommands_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to !commands$.
        /// </summary>
        internal static string GetCommands_Regex {
            get {
                return ResourceManager.GetString("GetCommands_Regex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to !raffle - Enter the current raffle.
        /// </summary>
        internal static string Raffle_Description {
            get {
                return ResourceManager.GetString("Raffle_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ^!raffle$.
        /// </summary>
        internal static string Raffle_Regex {
            get {
                return ResourceManager.GetString("Raffle_Regex", resourceCulture);
            }
        }
    }
}
