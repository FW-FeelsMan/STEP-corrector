   M S B A M L   `   `   `  ����  STEP-corrector    STEP_corrector.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   U QWpfAnimatedGif, Version=2.0.0.0, Culture=neutral, PublicKeyToken=9e7cd3b544a090dc,gif"http://wpfanimatedgif.codeplex.com  5   	   -   5          �Name$  STEP_corrector��5   	    � Title$ STEP-corrector��5	   	   $	��450��6    $	��800��6-    � AllowsTransparency .  � Icon$/ )pack://siteoforigin:,,,/img/faviconV2.png��6T    � 
ResizeMode$ NoResize=�5
   	    � WindowStyle$
 None=�6    � WindowStartupLocation$ CenterScreen=�62   ��5      �� 5   
    �� ImageSource$0 *pack://siteoforigin:,,,/img/background.png��6   .��5      � -   5         �Name$ MainGrid��5      .+�5   
   �� ���#031C30$
��Left=�6#   $	��452��6>   $��0,-1,0,-1.4q�6K   $	��Top=�6`   $��49��6x   �� 5   
   -   5        	 ��Name$	 STPbuttonSidebar��5      
 � ToolTip$&
  Редактирование STP��6+   $
��Left=�5   	   $��6,51,0,0q�6$   $	��Top=�5   	   $��34��6!   $��38��6,   ���	#00707070���	#00000000��5      �� 5       �� 
TargetType" ��6   .�5      ��  �� Property �@�OverridesDefaultStyle �� Value . 6>   �� 5       � �� 5      �� 5      $2 ,pack://siteoforigin:,,,/img/STPbuttonIco.png��6)   5      �� 5       � ��$ 0q�68   �� 5       � �� 5       �� 5!       �� 
TargetType" ��6.   .R�5"   "   � .+�5$   &   �� #��z��6-   5%   #    �� Triggers 5'   "   P� 5(   &    P� Property �@J�IsMouseOver P� Value . . �5)   *   ��  � �� 5*   .   �� 5+   2   $7 1pack://siteoforigin:,,,/img/STPbuttonIcoHover.png��6=   5-   +   5.   '   50      52      53      55      �� 57   
   -   5        $	 KMPSbuttonSidebar��57      $B
 <Редактирование 3Д-моделей Компас��6,   $
��Left=�58   	   $��	3,111,0,0q�6$   $	��Top=�59   	   $��42��6!   $��43��6,   ���	#00707070���	#00000000��5;      �� 5<      " ��6   .�5>      ��  �@�OverridesDefaultStyle . 6>   �� 5?       � �� 5@      �� 5A      $/ )pack://siteoforigin:,,,/img/kompasIco.png��6)   5C      �� 5D       � ��$ 0q�68   �� 5E       � �� 5F      �� 5G      " ��6.   .R�5H   "   � .+�5J   &   �� #��z��6-   5K   #    5M   "   P� 5N   &    �@J�IsMouseOver . . �5O   *   ��  � �� 5P   .   �� 5Q   2   $4 .pack://siteoforigin:,,,/img/kompasIcoHover.png��6=   5S   +   5T   '   5V      5X      5Y      5[      �� 5^   
   
�������$
��Left=�6!   $��26��6<   $	��Top=�6H   $	��800��6`   �� 5_   
   -   5         ��Name$ 
TitelLabel��5_      $ ��STEP-corrector (beta v7.0)��6$   $
��Left=�6I   $	��Top=�6d   ���	#FF030303$��49,0,0,0q�6�   
������ $	��153��6�   �� 5`   
   $
��Left=�6   $��23��6+   $��12,2,0,0q�67   $	��Top=�6I   $��24��6a   $+��%pack://siteoforigin:,,,/img/Union.png��6l   �� 5a   
   -   5        $	 
ButtonQuit��5a   <      CloseButtonStyle#���   6   $��X��6P   $
��Left=�5b      $��28��64   $��0,0,0,0q�6?   }�.  J� SnapsToDevicePixels . $��1��5c   4   
�������$��20(�6Y   $��Corbel)�6g   $��25��5d      $��	772,0,0,0q�6%   $	��Top=�68   $
 
Выход��5e      �� 5g   
   -   5        $	 ButtonCollaps��5g   <   #���   6   $��__��6S   $
��Left=�5h      $��29��64   $��0,0,0,0q�6?   }�.  . $��1��5i   4   
�������$��20(�6Y   $��Corbel)�6g   $��25��5j      $��	742,0,0,0q�6%   $	��Top=�68   $
 Свернуть��5k      �� 5m   
   ���	#FFC3C3C3$
��Left=�6%   $��1��6@   $��	58,26,0,0q�6K   $	��Top=�6^   $	��742��6v   �� 5n   
   $��32��6   $��0,0,126,4.6q�6   $��Bottom=�61   $��32��6L   $+��%pack://siteoforigin:,,,/img/Union.png��6W   $��Right=�6�   �� 5o   
   $��Группа ПТК��6   $
��Left=�6%   $��674,411,0,0q�6@   $	��Top=�6U   $
��Bold"�6m   $��Cambria Math)�6   ���	#00000000
���  ��6�   �� 5p   
   $s��mПРОМЫШЛЕННО-ИНЖИНИРИНГОВЫЙ ХОЛДИНГ В ТРАНСПОРТНОЙ ОТРАСЛИ��6   $
��Left=�5q      $��680,429,0,0q�5r      $	��Top=�5s      $��6(�5t      $
��Wrap=�5u      � 5x   
   -   5        $ STEPgrid��5x      $��
0,0,0,-0.4q�6!    J� OpacityMask
 �   �.+�5z      �� -	   5        $	 
ButtonList��5z   @   #���   6   $
��Left=�5{      $��22��6(   $��0,0,0,0q�63   }�.  . $��1��5|   (   
�������$��20(�6M   $��Corbel)�6[   $��25��5}      $��	720,0,0,0q�6   $	��Top=�6,   $6
 0Открыть список автозамены��6D   ��5~      �� 5      $. (pack://siteoforigin:,,,/img/listIcon.png��6!   5�      �� 5�      -
   5        $	 ButtonLoadSTP��5�        ImportButtonStyle#���  6,   $��Загрузить STEP��6X   $
��Left=�6q   $��124,283,0,0q�5�      $	��Top=�6.   $	��200��5�      $��60��6%   $��20(�5�      $��
Constantia)�6'   6?   �� 5�      $
��Left=�6   $��148,110,0,0q�6/   $	��Top=�6D   $	��150��6\   $0��*pack://siteoforigin:,,,/img/folderIcon.png��6i   �� 5�      
���  ��
���  ��$��3��68   $	��300��6B   $��390,70,407,80q�6O   �� 5�      $)��#Исправить STEP-файлы:��6   $
��Left=�64   $��
434,80,0,0q�6O   $	��Top=�6c   $��18(�6{   $��
Constantia)�6�   $Y
 SОтметьте файлы, которые необходимо исправить��5�   \   5�      �� 5�      -   5         ��Name$ FilesSTPListBox��5�      
�������$��1q�6B   $
��Left=�6V   $	��260��6q   $��440,129,0,191q�6}    �� SelectionMode$ Single=�6�   ��5�      �� 5�      .R�5�      ��  �� Orientation$ 
Horizontal=�6%   $��0q�6>   .+�5�      �� $��5,0,10,0q�6G    d� 	IsChecked 6'   �� *	IsChecked+6G   �� 5�      $��Center=�6B   ��6(   �� *
FileName+6B   �� 5�      $��10,0,0,0q�6G   $��Center=�6Y   
���������6(   �� *FileExtension+6t   5�      5�      5�      �� 5�      -   5        $	 ButtonFixSTP��5�      #���  6+   $��Исправить��6W   $
��Left=�6k   $��440,283,0,0q�5�      $	��Top=�6.   
�������$	��200��5�   ,   $��60��68   $��20(�5�      $��
Constantia)�6'   6?   �� 5�      -   5        $	 ButtonSelectAllSTP��5�      #���  61   $��Выбрать всё��6]   $
��Left=�6s   $��708,129,0,0q�5�      $	��Top=�6.   
�������$��85��5�   ,   $��30��67   $��
Constantia)�6C   $(
 "Отметить все файлы��5�      �� 5�      -   5        $	 ButtonUnSelectSTP��5�      #���  60   $��Отменить всё��6\   $
��Left=�6s   $��708,163,0,0q�5�      $	��Top=�6.   
�������$��85��5�   ,   $��30��67   $��
Constantia)�6C   $K
 EСнять выбор со всех отмеченных файлов��5�      �� 5�      -   5        $	 ButtonClearSTPlist��5�      #���  61   $��Очистить��6]   $
��Left=�6p   $��708,230,0,0q�5�      $	��Top=�6.   
�������$��85��5�   ,   $��30��67   $��
Constantia)�6C   $#
 Очистить список��5�      �� 5�      -   5        $	 ButtonExcludeSTP��5�      #���  6/   $��Исключить��6[   $
��Left=�6o   $��708,197,0,0q�5�      $	��Top=�6.   
�������$��85��5�   ,   $��30��67   $��
Constantia)�6C   $F
 @Удалить из списка отмеченные файлы��5�      �� 5�      $'��!Выберите STEP-файлы:��6   $
��Left=�63   $��
113,80,0,0q�6N   $	��Top=�6b   $��18(�6z   $��
Constantia)�6�   �� 5�      
�������$
��Left=�6%   $��37��6@   $��0,51,0,0q�6L   $	��Top=�6^   $��2��6v   5�      � 5�   
   -   5        $ 
STEPkompas��5�      $��
0,0,0,-0.4q�6#   
 �   �.+�5�      �� -   5        $	 ButtonLoadKompas��5�      #���  6/   $*��$Загрузить ЗD-модели:��6[   $
��Left=�6z   $��118,283,0,0q�5�      $	��Top=�6&   $	��206��5�      $��60��6   $��20(�5�      $��
Constantia)�6   67   �� 5�      -   5         ��Name$ kompasImgLabel��5�      $
��Left=�6,   $��
130,93,0,0q�6G   $	��Top=�6[   $	��185��6s   $4��.pack://siteoforigin:,,,/img/kompasIcoLabel.png��6�   $	��185��6�   �� 5�      
���  ��
���  ��$��3��68   $	��300��6B   $��390,70,407,80q�6O   �� 5�      $0��*Исправить обозначения:��6   $
��Left=�65   $��
434,80,0,0q�6P   $	��Top=�6d   $��18(�6|   $��
Constantia)�6�   $Y
 SОтметьте файлы, которые необходимо исправить��5�   \   5�      �� 5�      -   5        $ ModelListBox��5�      
�������$��1q�6?   $
��Left=�6S   $	��260��6n   $��440,129,0,191.4q�6z   $ Single=�6�   ��5�      �� 5�      .R�5�      �� $ 
Horizontal=�6%   $��0q�6>   .+�5�      �� $��5,0,10,0q�6G    6'   �� *	IsChecked+6G   �� 5�      $��Center=�6B   ��6(   �� *
FileName+6B   �� 5�      $��10,0,0,0q�6G   $��Center=�6Y   
���������6(   �� *FileExtension+6t   5�      5�      5�      �� 5�      -   5        $	 ButtonFixModel��5�      #���  6-   $��Исправить��6Y   $
��Left=�6m   $��440,283,0,0q�5�      $	��Top=�6&   
�������$	��200��5�   $   $��60��60   $��20(�5�      $��
Constantia)�6   67   �� 5�      -   5        $	 GetAllModelButton��5�      #���  60   $��Выбрать всё��6\   $
��Left=�6r   $��708,129,0,0q�5�      $	��Top=�6&   
�������$��85��5�   $   $��30��6/   $��
Constantia)�6;   $(
 "Отметить все файлы��5�      �� 5�      -   5        $	 unselectAllModelButton��5�      #���  65   $��Отменить всё��6a   $
��Left=�6x   $��708,163,0,0q�5�      $	��Top=�6&   
�������$��85��5�   $   $��30��6/   $��
Constantia)�6;   $K
 EСнять выбор со всех отмеченных файлов��5�      �� 5�      -   5        $	 ButtonClearListModel��5�      #���  63   $��Очистить��6_   $
��Left=�6r   $��708,230,0,0q�5�      $	��Top=�6&   
�������$��85��5�   $   $��30��6/   $��
Constantia)�6;   $#
 Очистить список��5�      �� 5�      -   5        $	 ButtonExcludeModel��5�      #���  61   $��Исключить��6]   $
��Left=�6q   $��708,197,0,0q�5�      $	��Top=�6&   
�������$��85��5�   $   $��30��6/   $��
Constantia)�6;   $F
 @Удалить из списка отмеченные файлы��5�      �� 5�      $(��"Выберите ЗD-модели:��6   $
��Left=�62   $��
126,80,0,0q�6M   $	��Top=�6a   $��18(�6y   $��
Constantia)�6�   �� 5�      -   5         ��Name$ flagSidebar��5�      
�������$
��Left=�6:   $��37��6U   $��	0,113,0,0q�6a   $	��Top=�6t   $��2��6�   5�      5�      5�      