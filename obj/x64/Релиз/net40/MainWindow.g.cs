﻿#pragma checksum "..\..\..\..\MainWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "C9E6D690272726B379CD228AFA5695C1CA4422FF"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using WpfAnimatedGif;


namespace STEP_corrector {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 1 "..\..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal STEP_corrector.MainWindow STEP_corrector;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid MainGrid;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label TitelLabel;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonQuit;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonCollaps;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonAutocorrecrList;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox FilesListBox;
        
        #line default
        #line hidden
        
        
        #line 112 "..\..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonClear;
        
        #line default
        #line hidden
        
        
        #line 116 "..\..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonExclude;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/STEP-corrector;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.STEP_corrector = ((STEP_corrector.MainWindow)(target));
            return;
            case 2:
            this.MainGrid = ((System.Windows.Controls.Grid)(target));
            
            #line 16 "..\..\..\..\MainWindow.xaml"
            this.MainGrid.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Header_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 3:
            this.TitelLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.ButtonQuit = ((System.Windows.Controls.Button)(target));
            
            #line 28 "..\..\..\..\MainWindow.xaml"
            this.ButtonQuit.Click += new System.Windows.RoutedEventHandler(this.ButtonQuit_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.ButtonCollaps = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\..\..\MainWindow.xaml"
            this.ButtonCollaps.Click += new System.Windows.RoutedEventHandler(this.ButtonCollaps_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.ButtonAutocorrecrList = ((System.Windows.Controls.Button)(target));
            
            #line 38 "..\..\..\..\MainWindow.xaml"
            this.ButtonAutocorrecrList.Click += new System.Windows.RoutedEventHandler(this.ButtonAutocorrecrList_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 65 "..\..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.FilesListBox = ((System.Windows.Controls.ListBox)(target));
            
            #line 76 "..\..\..\..\MainWindow.xaml"
            this.FilesListBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.FilesListBox_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 91 "..\..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_3);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 96 "..\..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_1);
            
            #line default
            #line hidden
            return;
            case 11:
            
            #line 101 "..\..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_2);
            
            #line default
            #line hidden
            return;
            case 12:
            this.ButtonClear = ((System.Windows.Controls.Button)(target));
            
            #line 115 "..\..\..\..\MainWindow.xaml"
            this.ButtonClear.Click += new System.Windows.RoutedEventHandler(this.Button_Clear);
            
            #line default
            #line hidden
            return;
            case 13:
            this.ButtonExclude = ((System.Windows.Controls.Button)(target));
            
            #line 119 "..\..\..\..\MainWindow.xaml"
            this.ButtonExclude.Click += new System.Windows.RoutedEventHandler(this.Button_Exclude);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

