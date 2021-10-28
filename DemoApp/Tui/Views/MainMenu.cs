using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Terminal.Gui;
using Terminal.Gui.Graphs;

namespace DemoApp.Tui.Views
{
    public sealed class MainMenu: FrameView
    {
        private readonly string[] _menuItems = new[] { "Authentication", "Collections" };
        private readonly ListView _menuListView;
        public MainMenu()
        {
            // Setup Window
            X = 0;
            Y = 1;
            Width = 20;
            Height = Dim.Fill();
            CanFocus = false;
            Shortcut = Key.CtrlMask | Key.C;
            Title = $"{Title} ({ShortcutTag})";
            ShortcutAction = SetFocus;

            // Create Left Bar
            _menuListView = new ListView (_menuItems) {
                                                          X = 0,
                                                          Y = 0,
                                                          Width = Dim.Fill (0),
                                                          Height = Dim.Fill (0),
                                                          AllowsMarking = false,
                                                          CanFocus = true,
                                                      };
            
            Add(_menuListView);
            
            
        }
    }
}