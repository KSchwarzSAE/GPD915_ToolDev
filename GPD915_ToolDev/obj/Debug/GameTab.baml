   M S B A M L   `   `   `  ����I  EGPD915_ToolDev, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:GPD915_ToolDevGPD915_ToolDev  5          GPD915_ToolDev.GameTab   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .localclr-namespace:GPD915_ToolDev     5        � 	Resources  5	      )   #GPD915_ToolDev.BoolToBrushConverter   	TrueBrush   
FalseBrush   achievementConverter��%7   &	          5
   
   
 �/kU�
 �����5      5      .��5      � -   5         �Name$
 grid��5       � ColumnDefinitions 5   
   �� 5      �� 5      �� 5      �� 5      �� 5       � RowDefinitions 5   
   �� 5      �� 5      �� 5      �� 5      �� 5      .+�5   
   �� -   5         ��Name$ gameNameLabel��5      $��1��5       $��2��6,   $��0��5!      ��5      ��  �� Path$ GameSettings.GameDir�5!      �� 5"   
   -   5         ��Name$ LaunchOrInstallButton��5"      $��Launch��5#      $��2��5$      $��2��6-   $��3��5%      5&      �� 5'   
   -   5        	 ��Name$	 	avmntList��5)      $��1��5'      $��3��6#   $��1��5(      $��2��6    $��5q�61   ��5*      �� 5+      .R�5,      � .+�5-      �� ��5.      �� $
 Name���5/      �� $ Achieved�
 �� 	Converter#
 [  50      51      53      � 54   
   -   5         �Name$ updateProgressBar��54      $��2��55      $��4��6&   $��2��56      $
��0, 5q�57      58      59      