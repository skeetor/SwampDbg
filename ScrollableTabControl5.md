# Q1: Add to this tabcontrol a left and right button in the header which srcolls the tabs if they dont fit in the window. <TabControl x:Class="WpfDockingManager.DragTabControl" 			xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 			xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 			xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"  			xmlns:d="http://schemas.microsoft.com/expression/blend/2008"  			xmlns:local="clr-namespace:WpfDockingManager" 			x:Name="dragTabControl" 			mc:Ignorable="d"  			d:DesignHeight="450" d:DesignWidth="800" 			AllowDrop="True" 			PreviewMouseLeftButtonDown="OnPreviewMouseLeftButtonDown" 			PreviewMouseMove="OnPreviewMouseMove" 			DragEnter="OnDragEnter" 			DragOver="OnDragOver" 			Drop="OnDrop">  	<TabControl.Resources> 		<local:TabItemRotation x:Key="TabItemRotation"/> 		<local:DockOrientationConverter x:Key="DockOrientationConverter"/>         <local:DockOrientationVisibillityConverter x:Key="DockOrientationVisibillity"/> 		<local:GridRowAdjustmentConverter x:Key="GridRowAdjustment"/>  		<Style x:Key="CloseButtonStyle" TargetType="Button"> 			<Setter Property="Background" Value="Transparent"/> 			<Setter Property="BorderThickness" Value="0"/> 			<Setter Property="HorizontalAlignment" Value="Center"/> 			<Setter Property="VerticalAlignment" Value="Center"/> 			<Setter Property="Width" Value="16"/> 			<Setter Property="Height" Value="16"/> 			<Setter Property="Template"> 				<Setter.Value> 					<ControlTemplate TargetType="Button"> 						<Grid> 							<Path x:Name="Cross" Stroke="Gray" StrokeThickness="2"  								Data="M4,4 L13,13 M4,13 L13,4"/> 						</Grid> 						<ControlTemplate.Triggers> 							<Trigger Property="IsMouseOver" Value="True"> 								<Setter TargetName="Cross" Property="Stroke" Value="Black"/> 							</Trigger> 						</ControlTemplate.Triggers> 					</ControlTemplate> 				</Setter.Value> 			</Setter> 			<EventSetter Event="Button.Click" Handler="OnCloseButtonEvent" /> 		</Style>  		<Style TargetType="{x:Type TabItem}"> 			<Setter Property="HeaderTemplate"> 				<Setter.Value> 					<DataTemplate> 						<ContentPresenter Content="{TemplateBinding Content}"> 							<ContentPresenter.LayoutTransform> 								<RotateTransform Angle="{Binding ElementName=dragTabControl, Converter={StaticResource TabItemRotation}, Path=TabStripPlacement}" /> 							</ContentPresenter.LayoutTransform> 						</ContentPresenter> 					</DataTemplate> 				</Setter.Value> 			</Setter> 			<Setter Property="Padding" Value="3" /> 		</Style> 	</TabControl.Resources>  	<TabControl.ItemContainerStyle> 		<Style TargetType="TabItem" BasedOn="{StaticResource {x:Type TabItem}}"> 			<Style.Triggers> 				<DataTrigger Binding="{Binding RelativeSource={RelativeSource Self}, Path=TabStripPlacement, Converter={StaticResource ResourceKey=DockOrientationConverter}}" Value="Horizontal"> 					<Setter Property="Height" Value="{Binding TabHeight, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}"/> 					<Setter Property="Template"> 						<Setter.Value> 							<ControlTemplate TargetType="TabItem"> 								<Grid> 									<Border x:Name="Border"  											Background="{TemplateBinding Background}" 											BorderThickness="{TemplateBinding BorderThickness}" 											BorderBrush="{TemplateBinding BorderBrush}" 											CornerRadius="2" 											Margin="2,2,2,0"> 										<Grid Name="_btnGrid"> 											<Grid.ColumnDefinitions> 												<ColumnDefinition Width="*"/> 												<ColumnDefinition Width="Auto"/> 											</Grid.ColumnDefinitions>  											<ContentPresenter x:Name="ContentSite" 																Grid.Column="0" 																VerticalAlignment="Center" 																HorizontalAlignment="Center" 																ContentSource="Header" 																RecognizesAccessKey="True" 																Margin="5,2,5,2"/>  											<Button x:Name="CloseButton" 														Grid.Column="1" 														Style="{StaticResource CloseButtonStyle}" 														Margin="5,0,5,0" 														Command="{Binding CloseTabCommand, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" 														CommandParameter="{Binding}"/> 										</Grid> 									</Border> 								</Grid> 								<ControlTemplate.Triggers> 									<Trigger Property="IsSelected" Value="True"> 										<Setter TargetName="Border" Property="Background" Value="{DynamicResource {x:Static SystemColors.WindowBrushKey}}"/> 										<Setter TargetName="Border" Property="BorderBrush" Value="DarkGray"/> 									</Trigger> 									<Trigger Property="IsSelected" Value="False"> 										<Setter TargetName="Border" Property="Background" Value="LightGray"/> 										<Setter TargetName="Border" Property="BorderBrush" Value="Gray"/> 									</Trigger> 									<Trigger Property="IsMouseOver" Value="True"> 										<Setter TargetName="Border" Property="Background" Value="#FFDDDDDD"/> 									</Trigger> 								</ControlTemplate.Triggers> 							</ControlTemplate> 						</Setter.Value> 					</Setter> 				</DataTrigger> 				<DataTrigger Binding="{Binding RelativeSource={RelativeSource Self}, Path=TabStripPlacement, Converter={StaticResource ResourceKey=DockOrientationConverter}}" Value="Vertical"> 					<Setter Property="Width" Value="{Binding TabHeight, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}"/> 					<Setter Property="Template"> 						<Setter.Value> 							<ControlTemplate TargetType="TabItem"> 								<Grid> 									<Border x:Name="Border"  											Background="{TemplateBinding Background}" 											BorderThickness="{TemplateBinding BorderThickness}" 											BorderBrush="{TemplateBinding BorderBrush}" 											CornerRadius="2" 											Margin="2,2,2,0"> 										<Grid Name="_btnGrid"> 											<Grid.RowDefinitions> 												<RowDefinition Height="Auto"/> 												<RowDefinition Height="*"/> 											</Grid.RowDefinitions>  											<ContentPresenter x:Name="ContentSite" 																Grid.Row="{Binding 																	RelativeSource={RelativeSource TemplatedParent}, 																	Path=TabStripPlacement, 																	Converter={StaticResource ResourceKey=GridRowAdjustment}, 																	ConverterParameter=content}" 																VerticalAlignment="Center" 																HorizontalAlignment="Center" 																ContentSource="Header" 																RecognizesAccessKey="True" 																Margin="5,2,5,2"/>  											<Button x:Name="CloseButton" 														Grid.Row="{Binding 															RelativeSource={RelativeSource TemplatedParent}, 															Path=TabStripPlacement, 															Converter={StaticResource ResourceKey=GridRowAdjustment}, 															ConverterParameter=button}" 														Style="{StaticResource CloseButtonStyle}" 														Margin="5,0,5,0" 														Command="{Binding CloseTabCommand, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" 														CommandParameter="{Binding}"/> 										</Grid> 									</Border> 								</Grid> 								<ControlTemplate.Triggers> 									<Trigger Property="IsSelected" Value="True"> 										<Setter TargetName="Border" Property="Background" Value="{DynamicResource {x:Static SystemColors.WindowBrushKey}}"/> 										<Setter TargetName="Border" Property="BorderBrush" Value="DarkGray"/> 									</Trigger> 									<Trigger Property="IsSelected" Value="False"> 										<Setter TargetName="Border" Property="Background" Value="LightGray"/> 										<Setter TargetName="Border" Property="BorderBrush" Value="Gray"/> 									</Trigger> 									<Trigger Property="IsMouseOver" Value="True"> 										<Setter TargetName="Border" Property="Background" Value="#FFDDDDDD"/> 									</Trigger> 								</ControlTemplate.Triggers> 							</ControlTemplate> 						</Setter.Value> 					</Setter> 				</DataTrigger> 			</Style.Triggers> 			<Setter Property="Width" Value="{Binding TabWidth, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}"/> 		</Style>  	</TabControl.ItemContainerStyle>  </TabControl>
To add left and right scroll buttons to the `TabControl` header when the tabs don't fit in the window, you can use a `ScrollViewer` and style the `TabPanel` to enable horizontal scrolling. Here's how you can modify your XAML to achieve this:

```xml
<TabControl
    x:Class="WpfDockingManager.DragTabControl"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:local="clr-namespace:WpfDockingManager"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    x:Name="dragTabControl"
    AllowDrop="True"
    d:DesignHeight="450"
    d:DesignWidth="800"
    DragEnter="OnDragEnter"
    DragOver="OnDragOver"
    Drop="OnDrop"
    mc:Ignorable="d"
    PreviewMouseLeftButtonDown="OnPreviewMouseLeftButtonDown"
    PreviewMouseMove="OnPreviewMouseMove">
    <TabControl.Resources>
        <local:TabItemRotation x:Key="TabItemRotation" />
        <local:DockOrientationConverter x:Key="DockOrientationConverter" />
        <local:DockOrientationVisibillityConverter x:Key="DockOrientationVisibillity" />
        <local:GridRowAdjustmentConverter x:Key="GridRowAdjustment" />

        <Style x:Key="CloseButtonStyle" TargetType="Button">
            <Setter Property="Background" Value="Transparent" />
            <Setter Property="BorderThickness" Value="0" />
            <Setter Property="HorizontalAlignment" Value="Center" />
            <Setter Property="VerticalAlignment" Value="Center" />
            <Setter Property="Width" Value="16" />
            <Setter Property="Height" Value="16" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="Button">
                        <Grid>
                            <Path
                                x:Name="Cross"
                                Data="M4,4 L13,13 M4,13 L13,4"
                                Stroke="Gray"
                                StrokeThickness="2" />
                        </Grid>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver" Value="True">
                                <Setter TargetName="Cross" Property="Stroke" Value="Black" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
            <EventSetter Event="Button.Click" Handler="OnCloseButtonEvent" />
        </Style>

        <Style TargetType="{x:Type TabItem}">
            <Setter Property="HeaderTemplate">
                <Setter.Value>
                    <DataTemplate>
                        <ContentPresenter Content="{TemplateBinding Content}">
                            <ContentPresenter.LayoutTransform>
                                <RotateTransform
                                    Angle="{Binding ElementName=dragTabControl, Converter={StaticResource TabItemRotation}, Path=TabStripPlacement}" />
                            </ContentPresenter.LayoutTransform>
                        </ContentPresenter>
                    </DataTemplate>
                </Setter.Value>
            </Setter>
            <Setter Property="Padding" Value="3" />
        </Style>
    </TabControl.Resources>

    <!--  Modified Template  -->
    <TabControl.Template>
        <ControlTemplate TargetType="TabControl">
            <Grid>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto" />
                    <RowDefinition Height="*" />
                </Grid.RowDefinitions>

                <!--  Custom TabPanel with ScrollViewer  -->
                <Border
                    Background="{TemplateBinding Background}"
                    BorderBrush="{TemplateBinding BorderBrush}"
                    BorderThickness="{TemplateBinding BorderThickness}">
                    <ScrollViewer
                        HorizontalScrollBarVisibility="Auto"
                        VerticalScrollBarVisibility="Hidden">
                        <TabPanel
                            x:Name="TabPanel"
                            Background="Transparent"
                            IsItemsHost="True"
                            Margin="0" />
                    </ScrollViewer>
                </Border>

                <!--  ContentPresenter  -->
                <ContentPresenter
                    Grid.Row="1"
                    ContentSource="SelectedContent" />
            </Grid>
        </ControlTemplate>
    </TabControl.Template>

    <TabControl.ItemContainerStyle>
        <Style BasedOn="{StaticResource {x:Type TabItem}}" TargetType="TabItem">
            <Style.Triggers>
                <DataTrigger
                    Binding="{Binding RelativeSource={RelativeSource Self}, Path=TabStripPlacement, Converter={StaticResource ResourceKey=DockOrientationConverter}}"
                    Value="Horizontal">
                    <Setter
                        Property="Height"
                        Value="{Binding TabHeight, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" />
                    <Setter Property="Template">
                        <Setter.Value>
                            <ControlTemplate TargetType="TabItem">
                                <Grid>
                                    <Border
                                        x:Name="Border"
                                        Background="{TemplateBinding Background}"
                                        BorderBrush="{TemplateBinding BorderBrush}"
                                        BorderThickness="{TemplateBinding BorderThickness}"
                                        CornerRadius="2"
                                        Margin="2,2,2,0">
                                        <Grid Name="_btnGrid">
                                            <Grid.ColumnDefinitions>
                                                <ColumnDefinition Width="*" />
                                                <ColumnDefinition Width="Auto" />
                                            </Grid.ColumnDefinitions>

                                            <ContentPresenter
                                                x:Name="ContentSite"
                                                Grid.Column="0"
                                                ContentSource="Header"
                                                HorizontalAlignment="Center"
                                                Margin="5,2,5,2"
                                                RecognizesAccessKey="True"
                                                VerticalAlignment="Center" />

                                            <Button
                                                x:Name="CloseButton"
                                                Grid.Column="1"
                                                Command="{Binding CloseTabCommand, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}"
                                                CommandParameter="{Binding}"
                                                Margin="5,0,5,0"
                                                Style="{StaticResource CloseButtonStyle}" />
                                        </Grid>
                                    </Border>
                                </Grid>
                                <ControlTemplate.Triggers>
                                    <Trigger Property="IsSelected" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="{DynamicResource {x:Static SystemColors.WindowBrushKey}}" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="DarkGray" />
                                    </Trigger>
                                    <Trigger Property="IsSelected" Value="False">
                                        <Setter TargetName="Border" Property="Background" Value="LightGray" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="Gray" />
                                    </Trigger>
                                    <Trigger Property="IsMouseOver" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="#FFDDDDDD" />
                                    </Trigger>
                                </ControlTemplate.Triggers>
                            </ControlTemplate>
                        </Setter.Value>
                    </Setter>
                </DataTrigger>
                <DataTrigger
                    Binding="{Binding RelativeSource={RelativeSource Self}, Path=TabStripPlacement, Converter={StaticResource ResourceKey=DockOrientationConverter}}"
                    Value="Vertical">
                    <Setter
                        Property="Width"
                        Value="{Binding TabHeight, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" />
                    <Setter Property="Template">
                        <Setter.Value>
                            <ControlTemplate TargetType="TabItem">
                                <Grid>
                                    <Border
                                        x:Name="Border"
                                        Background="{TemplateBinding Background}"
                                        BorderBrush="{TemplateBinding BorderBrush}"
                                        BorderThickness="{TemplateBinding BorderThickness}"
                                        CornerRadius="2"
                                        Margin="2,2,2,0">
                                        <Grid Name="_btnGrid">
                                            <Grid.RowDefinitions>
                                                <RowDefinition Height="Auto" />
                                                <RowDefinition Height="*" />
                                            </Grid.RowDefinitions>

                                            <ContentPresenter
                                                x:Name="ContentSite"
                                                Grid.Row="{Binding 
                                                    RelativeSource={RelativeSource TemplatedParent}, 
                                                    Path=TabStripPlacement, 
                                                    Converter={StaticResource ResourceKey=GridRowAdjustment}, 
                                                    ConverterParameter=content}"
                                                ContentSource="Header"
                                                HorizontalAlignment="Center"
                                                Margin="5,2,5,2"
                                                RecognizesAccessKey="True"
                                                VerticalAlignment="Center" />

                                            <Button
                                                x:Name="CloseButton"
                                                Grid.Row="{Binding 
                                                    RelativeSource={RelativeSource TemplatedParent}, 
                                                    Path=TabStripPlacement, 
                                                    Converter={StaticResource ResourceKey=GridRowAdjustment}, 
                                                    ConverterParameter=button}"
                                                Command="{Binding CloseTabCommand, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}"
                                                CommandParameter="{Binding}"
                                                Margin="5,0,5,0"
                                                Style="{StaticResource CloseButtonStyle}" />
                                        </Grid>
                                    </Border>
                                </Grid>
                                <ControlTemplate.Triggers>
                                    <Trigger Property="IsSelected" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="{DynamicResource {x:Static SystemColors.WindowBrushKey}}" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="DarkGray" />
                                    </Trigger>
                                    <Trigger Property="IsSelected" Value="False">
                                        <Setter TargetName="Border" Property="Background" Value="LightGray" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="Gray" />
                                    </Trigger>
                                    <Trigger Property="IsMouseOver" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="#FFDDDDDD" />
                                    </Trigger>
                                </ControlTemplate.Triggers>
                            </ControlTemplate>
                        </Setter.Value>
                    </Setter>
                </DataTrigger>
            </Style.Triggers>
            <Setter
                Property="Width"
                Value="{Binding TabWidth, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" />
        </Style>
    </TabControl.ItemContainerStyle>

</TabControl>
```

Key changes and explanations:

1.  **`TabControl.Template`**: The template of the `TabControl` is redefined to include a `ScrollViewer`.
2.  **`ScrollViewer`**: A `ScrollViewer` is added to wrap the `TabPanel`.
    *   `HorizontalScrollBarVisibility="Auto"`: This ensures that the horizontal scrollbar appears only when the tabs exceed the visible width.
    *   `VerticalScrollBarVisibility="Hidden"`: This hides the vertical scrollbar since we only want horizontal scrolling.
3.  **`TabPanel`**: The `TabPanel` is set as the `ItemsHost` within the `ScrollViewer`. This is where the `TabItem` elements are rendered.
4.  **Layout**: The `Grid` within the `ControlTemplate` is set up with two rows: one for the tabs (with scrolling) and one for the content.

This setup will automatically add a horizontal scrollbar to the `TabControl` when the tabs don't fit within the available space, allowing the user to scroll through the tabs. [3](https://www.essentialobjects.com/doc/Wpf/controls/TabControl/scrolling.html)
# Q2: Add a left and right button in the header which do the scrolling
Okay, let's add left and right buttons to the `TabControl` header to handle the scrolling. This involves modifying the `TabControl` template to include these buttons and implementing the scrolling logic.

Here's the updated XAML:

```xml
<TabControl
    x:Class="WpfDockingManager.DragTabControl"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:local="clr-namespace:WpfDockingManager"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    x:Name="dragTabControl"
    AllowDrop="True"
    d:DesignHeight="450"
    d:DesignWidth="800"
    DragEnter="OnDragEnter"
    DragOver="OnDragOver"
    Drop="OnDrop"
    mc:Ignorable="d"
    PreviewMouseLeftButtonDown="OnPreviewMouseLeftButtonDown"
    PreviewMouseMove="OnPreviewMouseMove">
    <TabControl.Resources>
        <local:TabItemRotation x:Key="TabItemRotation" />
        <local:DockOrientationConverter x:Key="DockOrientationConverter" />
        <local:DockOrientationVisibillityConverter x:Key="DockOrientationVisibillity" />
        <local:GridRowAdjustmentConverter x:Key="GridRowAdjustment" />

        <Style x:Key="CloseButtonStyle" TargetType="Button">
            <Setter Property="Background" Value="Transparent" />
            <Setter Property="BorderThickness" Value="0" />
            <Setter Property="HorizontalAlignment" Value="Center" />
            <Setter Property="VerticalAlignment" Value="Center" />
            <Setter Property="Width" Value="16" />
            <Setter Property="Height" Value="16" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="Button">
                        <Grid>
                            <Path
                                x:Name="Cross"
                                Data="M4,4 L13,13 M4,13 L13,4"
                                Stroke="Gray"
                                StrokeThickness="2" />
                        </Grid>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver" Value="True">
                                <Setter TargetName="Cross" Property="Stroke" Value="Black" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
            <EventSetter Event="Button.Click" Handler="OnCloseButtonEvent" />
        </Style>

        <Style TargetType="{x:Type TabItem}">
            <Setter Property="HeaderTemplate">
                <Setter.Value>
                    <DataTemplate>
                        <ContentPresenter Content="{TemplateBinding Content}">
                            <ContentPresenter.LayoutTransform>
                                <RotateTransform
                                    Angle="{Binding ElementName=dragTabControl, Converter={StaticResource TabItemRotation}, Path=TabStripPlacement}" />
                            </ContentPresenter.LayoutTransform>
                        </ContentPresenter>
                    </DataTemplate>
                </Setter.Value>
            </Setter>
            <Setter Property="Padding" Value="3" />
        </Style>
    </TabControl.Resources>

    <!--  Modified Template  -->
    <TabControl.Template>
        <ControlTemplate TargetType="TabControl">
            <Grid>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto" />
                    <RowDefinition Height="*" />
                </Grid.RowDefinitions>

                <!--  Custom TabPanel with Scroll Buttons  -->
                <Border
                    Background="{TemplateBinding Background}"
                    BorderBrush="{TemplateBinding BorderBrush}"
                    BorderThickness="{TemplateBinding BorderThickness}">
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="Auto" />
                            <ColumnDefinition Width="*" />
                            <ColumnDefinition Width="Auto" />
                        </Grid.ColumnDefinitions>

                        <!--  Left Scroll Button  -->
                        <Button
                            x:Name="LeftScrollButton"
                            Click="LeftScrollButton_Click"
                            Content="&lt;"
                            FontSize="16"
                            FontWeight="Bold"
                            Margin="2"
                            Padding="5,0"
                            Visibility="Collapsed" />

                        <!--  ScrollViewer for Tabs  -->
                        <ScrollViewer
                            x:Name="TabScrollViewer"
                            Grid.Column="1"
                            HorizontalScrollBarVisibility="Hidden"
                            VerticalScrollBarVisibility="Hidden"
                            ScrollChanged="TabScrollViewer_ScrollChanged">
                            <TabPanel
                                x:Name="TabPanel"
                                Background="Transparent"
                                IsItemsHost="True"
                                Margin="0" />
                        </ScrollViewer>

                        <!--  Right Scroll Button  -->
                        <Button
                            x:Name="RightScrollButton"
                            Grid.Column="2"
                            Click="RightScrollButton_Click"
                            Content=">"
                            FontSize="16"
                            FontWeight="Bold"
                            Margin="2"
                            Padding="5,0"
                            Visibility="Collapsed" />
                    </Grid>
                </Border>

                <!--  ContentPresenter  -->
                <ContentPresenter
                    Grid.Row="1"
                    ContentSource="SelectedContent" />
            </Grid>
            <ControlTemplate.Triggers>
                <Trigger Property="IsMouseOver" Value="True">
                    <Setter TargetName="LeftScrollButton" Property="Visibility" Value="Visible" />
                    <Setter TargetName="RightScrollButton" Property="Visibility" Value="Visible" />
                </Trigger>
            </ControlTemplate.Triggers>
        </ControlTemplate>
    </TabControl.Template>

    <TabControl.ItemContainerStyle>
        <Style BasedOn="{StaticResource {x:Type TabItem}}" TargetType="TabItem">
            <Style.Triggers>
                <DataTrigger
                    Binding="{Binding RelativeSource={RelativeSource Self}, Path=TabStripPlacement, Converter={StaticResource ResourceKey=DockOrientationConverter}}"
                    Value="Horizontal">
                    <Setter
                        Property="Height"
                        Value="{Binding TabHeight, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" />
                    <Setter Property="Template">
                        <Setter.Value>
                            <ControlTemplate TargetType="TabItem">
                                <Grid>
                                    <Border
                                        x:Name="Border"
                                        Background="{TemplateBinding Background}"
                                        BorderBrush="{TemplateBinding BorderBrush}"
                                        BorderThickness="{TemplateBinding BorderThickness}"
                                        CornerRadius="2"
                                        Margin="2,2,2,0">
                                        <Grid Name="_btnGrid">
                                            <Grid.ColumnDefinitions>
                                                <ColumnDefinition Width="*" />
                                                <ColumnDefinition Width="Auto" />
                                            </Grid.ColumnDefinitions>

                                            <ContentPresenter
                                                x:Name="ContentSite"
                                                Grid.Column="0"
                                                ContentSource="Header"
                                                HorizontalAlignment="Center"
                                                Margin="5,2,5,2"
                                                RecognizesAccessKey="True"
                                                VerticalAlignment="Center" />

                                            <Button
                                                x:Name="CloseButton"
                                                Grid.Column="1"
                                                Command="{Binding CloseTabCommand, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}"
                                                CommandParameter="{Binding}"
                                                Margin="5,0,5,0"
                                                Style="{StaticResource CloseButtonStyle}" />
                                        </Grid>
                                    </Border>
                                </Grid>
                                <ControlTemplate.Triggers>
                                    <Trigger Property="IsSelected" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="{DynamicResource {x:Static SystemColors.WindowBrushKey}}" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="DarkGray" />
                                    </Trigger>
                                    <Trigger Property="IsSelected" Value="False">
                                        <Setter TargetName="Border" Property="Background" Value="LightGray" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="Gray" />
                                    </Trigger>
                                    <Trigger Property="IsMouseOver" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="#FFDDDDDD" />
                                    </Trigger>
                                </ControlTemplate.Triggers>
                            </ControlTemplate>
                        </Setter.Value>
                    </Setter>
                </DataTrigger>
                <DataTrigger
                    Binding="{Binding RelativeSource={RelativeSource Self}, Path=TabStripPlacement, Converter={StaticResource ResourceKey=DockOrientationConverter}}"
                    Value="Vertical">
                    <Setter
                        Property="Width"
                        Value="{Binding TabHeight, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" />
                    <Setter Property="Template">
                        <Setter.Value>
                            <ControlTemplate TargetType="TabItem">
                                <Grid>
                                    <Border
                                        x:Name="Border"
                                        Background="{TemplateBinding Background}"
                                        BorderBrush="{TemplateBinding BorderBrush}"
                                        BorderThickness="{TemplateBinding BorderThickness}"
                                        CornerRadius="2"
                                        Margin="2,2,2,0">
                                        <Grid Name="_btnGrid">
                                            <Grid.RowDefinitions>
                                                <RowDefinition Height="Auto" />
                                                <RowDefinition Height="*" />
                                            </Grid.RowDefinitions>

                                            <ContentPresenter
                                                x:Name="ContentSite"
                                                Grid.Row="{Binding 
                                                    RelativeSource={RelativeSource TemplatedParent}, 
                                                    Path=TabStripPlacement, 
                                                    Converter={StaticResource ResourceKey=GridRowAdjustment}, 
                                                    ConverterParameter=content}"
                                                ContentSource="Header"
                                                HorizontalAlignment="Center"
                                                Margin="5,2,5,2"
                                                RecognizesAccessKey="True"
                                                VerticalAlignment="Center" />

                                            <Button
                                                x:Name="CloseButton"
                                                Grid.Row="{Binding 
                                                    RelativeSource={RelativeSource TemplatedParent}, 
                                                    Path=TabStripPlacement, 
                                                    Converter={StaticResource ResourceKey=GridRowAdjustment}, 
                                                    ConverterParameter=button}"
                                                Command="{Binding CloseTabCommand, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}"
                                                CommandParameter="{Binding}"
                                                Margin="5,0,5,0"
                                                Style="{StaticResource CloseButtonStyle}" />
                                        </Grid>
                                    </Border>
                                </Grid>
                                <ControlTemplate.Triggers>
                                    <Trigger Property="IsSelected" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="{DynamicResource {x:Static SystemColors.WindowBrushKey}}" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="DarkGray" />
                                    </Trigger>
                                    <Trigger Property="IsSelected" Value="False">
                                        <Setter TargetName="Border" Property="Background" Value="LightGray" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="Gray" />
                                    </Trigger>
                                    <Trigger Property="IsMouseOver" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="#FFDDDDDD" />
                                    </Trigger>
                                </ControlTemplate.Triggers>
                            </ControlTemplate>
                        </Setter.Value>
                    </Setter>
                </DataTrigger>
            </Style.Triggers>
            <Setter
                Property="Width"
                Value="{Binding TabWidth, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" />
        </Style>
    </TabControl.ItemContainerStyle>

</TabControl>
```

```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfDockingManager
{
    public partial class DragTabControl : TabControl
    {
        public DragTabControl()
        {
            InitializeComponent();
            this.Loaded += DragTabControl_Loaded;
        }

        private void DragTabControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateScrollButtonVisibility();
        }

        private void LeftScrollButton_Click(object sender, RoutedEventArgs e)
        {
            TabScrollViewer.ScrollToHorizontalOffset(TabScrollViewer.HorizontalOffset - 50);
            UpdateScrollButtonVisibility();
        }

        private void RightScrollButton_Click(object sender, RoutedEventArgs e)
        {
            TabScrollViewer.ScrollToHorizontalOffset(TabScrollViewer.HorizontalOffset + 50);
            UpdateScrollButtonVisibility();
        }

        private void TabScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            UpdateScrollButtonVisibility();
        }

        private void UpdateScrollButtonVisibility()
        {
            if (TabScrollViewer == null || LeftScrollButton == null || RightScrollButton == null)
                return;

            bool canScrollLeft = TabScrollViewer.HorizontalOffset > 0;
            bool canScrollRight = TabScrollViewer.HorizontalOffset < TabScrollViewer.ExtentWidth - TabScrollViewer.ViewportWidth;

            LeftScrollButton.Visibility = canScrollLeft ? Visibility.Visible : Visibility.Collapsed;
            RightScrollButton.Visibility = canScrollRight ? Visibility.Visible : Visibility.Collapsed;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateScrollButtonVisibility();
        }

        private void OnPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Your existing logic
        }

        private void OnPreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Your existing logic
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            // Your existing logic
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            // Your existing logic
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            // Your existing logic
        }

        private void OnCloseButtonEvent(object sender, RoutedEventArgs e)
        {
            // Your existing logic
        }
    }
}
```

Key improvements and explanations:

1.  **Template Modification**:
    *   The `TabControl` template is updated to include `LeftScrollButton`, `RightScrollButton`, and a `ScrollViewer` named `TabScrollViewer`.
    *   The `TabPanel` is placed inside the `ScrollViewer`.
    *   The scroll buttons are initially collapsed and made visible on mouse-over.
2.  **Scroll Buttons**:
    *   Left and right buttons are added with click event handlers.
    *   The content of the buttons is set to "<" and ">" for visual indication.
3.  **ScrollViewer**:
    *   The `ScrollViewer` is configured to hide both horizontal and vertical scrollbars.
    *   The `ScrollChanged` event is used to update the visibility of the scroll buttons.
4.  **Event Handlers**:
    *   `LeftScrollButton_Click` and `RightScrollButton_Click` scroll the `TabScrollViewer` horizontally.
    *   `TabScrollViewer_ScrollChanged` updates the visibility of the scroll buttons based on the current scroll position.
5.  **Visibility Logic**:
    *   The `UpdateScrollButtonVisibility` method checks if scrolling is possible in either direction and updates the button visibility accordingly.
    *   The `OnRenderSizeChanged` method ensures that the button visibility is updated when the control is resized.
6.  **Initial Setup**:
    *   The `DragTabControl_Loaded` event handler is used to call `UpdateScrollButtonVisibility` when the control is loaded.

To make this fully functional:

*   Ensure that the `DragTabControl` class in your code-behind inherits from `TabControl` and includes the event handlers (`LeftScrollButton_Click`, `RightScrollButton_Click`, `TabScrollViewer_ScrollChanged`, and `UpdateScrollButtonVisibility`).
*   Keep your existing logic for drag and drop, close buttons, and other custom behaviors.

This approach provides a clean and functional way to add scroll buttons to your `TabControl`, enhancing the user experience when dealing with a large number of tabs. [2](https://github.com/AvaloniaUI/Avalonia/discussions/19103)
# Q3: Visibillity of the scrollbuttons should depend on a converter
Okay, let's modify the visibility of the scroll buttons to depend on a converter. This approach provides more flexibility and cleaner separation of concerns. We'll create a converter that checks if scrolling is possible and returns the appropriate `Visibility` value.

First, create the converter:

```csharp
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace WpfDockingManager
{
    public class ScrollButtonVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 3 ||
                !(values[0] is double) ||
                !(values[1] is double) ||
                !(values[2] is double))
            {
                return Visibility.Collapsed;
            }

            double horizontalOffset = (double)values[0];
            double extentWidth = (double)values[1];
            double viewportWidth = (double)values[2];

            string direction = parameter as string;

            if (direction == "Left")
            {
                return (horizontalOffset > 0) ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (direction == "Right")
            {
                return (horizontalOffset < extentWidth - viewportWidth) ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
```

Next, update your XAML to use this converter:

```xml
<TabControl
    x:Class="WpfDockingManager.DragTabControl"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:local="clr-namespace:WpfDockingManager"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    x:Name="dragTabControl"
    AllowDrop="True"
    d:DesignHeight="450"
    d:DesignWidth="800"
    DragEnter="OnDragEnter"
    DragOver="OnDragOver"
    Drop="OnDrop"
    mc:Ignorable="d"
    PreviewMouseLeftButtonDown="OnPreviewMouseLeftButtonDown"
    PreviewMouseMove="OnPreviewMouseMove">
    <TabControl.Resources>
        <local:TabItemRotation x:Key="TabItemRotation" />
        <local:DockOrientationConverter x:Key="DockOrientationConverter" />
        <local:DockOrientationVisibillityConverter x:Key="DockOrientationVisibillity" />
        <local:GridRowAdjustmentConverter x:Key="GridRowAdjustment" />
        <local:ScrollButtonVisibilityConverter x:Key="ScrollButtonVisibilityConverter" />

        <Style x:Key="CloseButtonStyle" TargetType="Button">
            <Setter Property="Background" Value="Transparent" />
            <Setter Property="BorderThickness" Value="0" />
            <Setter Property="HorizontalAlignment" Value="Center" />
            <Setter Property="VerticalAlignment" Value="Center" />
            <Setter Property="Width" Value="16" />
            <Setter Property="Height" Value="16" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="Button">
                        <Grid>
                            <Path
                                x:Name="Cross"
                                Data="M4,4 L13,13 M4,13 L13,4"
                                Stroke="Gray"
                                StrokeThickness="2" />
                        </Grid>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver" Value="True">
                                <Setter TargetName="Cross" Property="Stroke" Value="Black" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
            <EventSetter Event="Button.Click" Handler="OnCloseButtonEvent" />
        </Style>

        <Style TargetType="{x:Type TabItem}">
            <Setter Property="HeaderTemplate">
                <Setter.Value>
                    <DataTemplate>
                        <ContentPresenter Content="{TemplateBinding Content}">
                            <ContentPresenter.LayoutTransform>
                                <RotateTransform
                                    Angle="{Binding ElementName=dragTabControl, Converter={StaticResource TabItemRotation}, Path=TabStripPlacement}" />
                            </ContentPresenter.LayoutTransform>
                        </ContentPresenter>
                    </DataTemplate>
                </Setter.Value>
            </Setter>
            <Setter Property="Padding" Value="3" />
        </Style>
    </TabControl.Resources>

    <!--  Modified Template  -->
    <TabControl.Template>
        <ControlTemplate TargetType="TabControl">
            <Grid>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto" />
                    <RowDefinition Height="*" />
                </Grid.RowDefinitions>

                <!--  Custom TabPanel with Scroll Buttons  -->
                <Border
                    Background="{TemplateBinding Background}"
                    BorderBrush="{TemplateBinding BorderBrush}"
                    BorderThickness="{TemplateBinding BorderThickness}">
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="Auto" />
                            <ColumnDefinition Width="*" />
                            <ColumnDefinition Width="Auto" />
                        </Grid.ColumnDefinitions>

                        <!--  Left Scroll Button  -->
                        <Button
                            x:Name="LeftScrollButton"
                            Click="LeftScrollButton_Click"
                            Content="&lt;"
                            FontSize="16"
                            FontWeight="Bold"
                            Margin="2"
                            Padding="5,0">
                            <Button.Visibility>
                                <MultiBinding Converter="{StaticResource ScrollButtonVisibilityConverter}" ConverterParameter="Left">
                                    <Binding ElementName="TabScrollViewer" Path="HorizontalOffset" />
                                    <Binding ElementName="TabScrollViewer" Path="ExtentWidth" />
                                    <Binding ElementName="TabScrollViewer" Path="ViewportWidth" />
                                </MultiBinding>
                            </Button.Visibility>
                        </Button>

                        <!--  ScrollViewer for Tabs  -->
                        <ScrollViewer
                            x:Name="TabScrollViewer"
                            Grid.Column="1"
                            HorizontalScrollBarVisibility="Hidden"
                            VerticalScrollBarVisibility="Hidden"
                            ScrollChanged="TabScrollViewer_ScrollChanged">
                            <TabPanel
                                x:Name="TabPanel"
                                Background="Transparent"
                                IsItemsHost="True"
                                Margin="0" />
                        </ScrollViewer>

                        <!--  Right Scroll Button  -->
                        <Button
                            x:Name="RightScrollButton"
                            Grid.Column="2"
                            Click="RightScrollButton_Click"
                            Content=">"
                            FontSize="16"
                            FontWeight="Bold"
                            Margin="2"
                            Padding="5,0">
                            <Button.Visibility>
                                <MultiBinding Converter="{StaticResource ScrollButtonVisibilityConverter}" ConverterParameter="Right">
                                    <Binding ElementName="TabScrollViewer" Path="HorizontalOffset" />
                                    <Binding ElementName="TabScrollViewer" Path="ExtentWidth" />
                                    <Binding ElementName="TabScrollViewer" Path="ViewportWidth" />
                                </MultiBinding>
                            </Button.Visibility>
                        </Button>
                    </Grid>
                </Border>

                <!--  ContentPresenter  -->
                <ContentPresenter
                    Grid.Row="1"
                    ContentSource="SelectedContent" />
            </Grid>
        </ControlTemplate>
    </TabControl.Template>

    <TabControl.ItemContainerStyle>
        <Style BasedOn="{StaticResource {x:Type TabItem}}" TargetType="TabItem">
            <Style.Triggers>
                <DataTrigger
                    Binding="{Binding RelativeSource={RelativeSource Self}, Path=TabStripPlacement, Converter={StaticResource ResourceKey=DockOrientationConverter}}"
                    Value="Horizontal">
                    <Setter
                        Property="Height"
                        Value="{Binding TabHeight, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" />
                    <Setter Property="Template">
                        <Setter.Value>
                            <ControlTemplate TargetType="TabItem">
                                <Grid>
                                    <Border
                                        x:Name="Border"
                                        Background="{TemplateBinding Background}"
                                        BorderBrush="{TemplateBinding BorderBrush}"
                                        BorderThickness="{TemplateBinding BorderThickness}"
                                        CornerRadius="2"
                                        Margin="2,2,2,0">
                                        <Grid Name="_btnGrid">
                                            <Grid.ColumnDefinitions>
                                                <ColumnDefinition Width="*" />
                                                <ColumnDefinition Width="Auto" />
                                            </Grid.ColumnDefinitions>

                                            <ContentPresenter
                                                x:Name="ContentSite"
                                                Grid.Column="0"
                                                ContentSource="Header"
                                                HorizontalAlignment="Center"
                                                Margin="5,2,5,2"
                                                RecognizesAccessKey="True"
                                                VerticalAlignment="Center" />

                                            <Button
                                                x:Name="CloseButton"
                                                Grid.Column="1"
                                                Command="{Binding CloseTabCommand, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}"
                                                CommandParameter="{Binding}"
                                                Margin="5,0,5,0"
                                                Style="{StaticResource CloseButtonStyle}" />
                                        </Grid>
                                    </Border>
                                </Grid>
                                <ControlTemplate.Triggers>
                                    <Trigger Property="IsSelected" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="{DynamicResource {x:Static SystemColors.WindowBrushKey}}" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="DarkGray" />
                                    </Trigger>
                                    <Trigger Property="IsSelected" Value="False">
                                        <Setter TargetName="Border" Property="Background" Value="LightGray" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="Gray" />
                                    </Trigger>
                                    <Trigger Property="IsMouseOver" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="#FFDDDDDD" />
                                    </Trigger>
                                </ControlTemplate.Triggers>
                            </ControlTemplate>
                        </Setter.Value>
                    </Setter>
                </DataTrigger>
                <DataTrigger
                    Binding="{Binding RelativeSource={RelativeSource Self}, Path=TabStripPlacement, Converter={StaticResource ResourceKey=DockOrientationConverter}}"
                    Value="Vertical">
                    <Setter
                        Property="Width"
                        Value="{Binding TabHeight, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" />
                    <Setter Property="Template">
                        <Setter.Value>
                            <ControlTemplate TargetType="TabItem">
                                <Grid>
                                    <Border
                                        x:Name="Border"
                                        Background="{TemplateBinding Background}"
                                        BorderBrush="{TemplateBinding BorderBrush}"
                                        BorderThickness="{TemplateBinding BorderThickness}"
                                        CornerRadius="2"
                                        Margin="2,2,2,0">
                                        <Grid Name="_btnGrid">
                                            <Grid.RowDefinitions>
                                                <RowDefinition Height="Auto" />
                                                <RowDefinition Height="*" />
                                            </Grid.RowDefinitions>

                                            <ContentPresenter
                                                x:Name="ContentSite"
                                                Grid.Row="{Binding 
                                                    RelativeSource={RelativeSource TemplatedParent}, 
                                                    Path=TabStripPlacement, 
                                                    Converter={StaticResource ResourceKey=GridRowAdjustment}, 
                                                    ConverterParameter=content}"
                                                ContentSource="Header"
                                                HorizontalAlignment="Center"
                                                Margin="5,2,5,2"
                                                RecognizesAccessKey="True"
                                                VerticalAlignment="Center" />

                                            <Button
                                                x:Name="CloseButton"
                                                Grid.Row="{Binding 
                                                    RelativeSource={RelativeSource TemplatedParent}, 
                                                    Path=TabStripPlacement, 
                                                    Converter={StaticResource ResourceKey=GridRowAdjustment}, 
                                                    ConverterParameter=button}"
                                                Command="{Binding CloseTabCommand, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}"
                                                CommandParameter="{Binding}"
                                                Margin="5,0,5,0"
                                                Style="{StaticResource CloseButtonStyle}" />
                                        </Grid>
                                    </Border>
                                </Grid>
                                <ControlTemplate.Triggers>
                                    <Trigger Property="IsSelected" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="{DynamicResource {x:Static SystemColors.WindowBrushKey}}" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="DarkGray" />
                                    </Trigger>
                                    <Trigger Property="IsSelected" Value="False">
                                        <Setter TargetName="Border" Property="Background" Value="LightGray" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="Gray" />
                                    </Trigger>
                                    <Trigger Property="IsMouseOver" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="#FFDDDDDD" />
                                    </Trigger>
                                </ControlTemplate.Triggers>
                            </ControlTemplate>
                        </Setter.Value>
                    </Setter>
                </DataTrigger>
            </Style.Triggers>
            <Setter
                Property="Width"
                Value="{Binding TabWidth, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" />
        </Style>
    </TabControl.ItemContainerStyle>

</TabControl>
```

```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfDockingManager
{
    public partial class DragTabControl : TabControl
    {
        public DragTabControl()
        {
            InitializeComponent();
        }

        private void LeftScrollButton_Click(object sender, RoutedEventArgs e)
        {
            TabScrollViewer.ScrollToHorizontalOffset(TabScrollViewer.HorizontalOffset - 50);
        }

        private void RightScrollButton_Click(object sender, RoutedEventArgs e)
        {
            TabScrollViewer.ScrollToHorizontalOffset(TabScrollViewer.HorizontalOffset + 50);
        }

        private void TabScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // No need to update visibility here, the converter will handle it
        }

        private void OnPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Your existing logic
        }

        private void OnPreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Your existing logic
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            // Your existing logic
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            // Your existing logic
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            // Your existing logic
        }

        private void OnCloseButtonEvent(object sender, RoutedEventArgs e)
        {
            // Your existing logic
        }
    }
}
```

Key changes and explanations:

1.  **ScrollButtonVisibilityConverter**:
    *   This `IMultiValueConverter` takes the `HorizontalOffset`, `ExtentWidth`, and `ViewportWidth` of the `ScrollViewer` as input.
    *   It also takes a `ConverterParameter` to determine whether it's checking for left or right scrolling capability.
    *   It returns `Visibility.Visible` if scrolling is possible in the specified direction, and `Visibility.Collapsed` otherwise.
2.  **XAML Binding**:
    *   The `Visibility` property of both `LeftScrollButton` and `RightScrollButton` is bound to a `MultiBinding` that uses the `ScrollButtonVisibilityConverter`.
    *   The `HorizontalOffset`, `ExtentWidth`, and `ViewportWidth` properties of the `TabScrollViewer` are bound to the `MultiBinding`.
    *   The `ConverterParameter` is set to "Left" for the left button and "Right" for the right button.
3.  **Code-Behind**:
    *   The `UpdateScrollButtonVisibility` method and `OnRenderSizeChanged` override are no longer needed, as the converter handles the visibility logic.
    *   The `TabScrollViewer_ScrollChanged` event handler is simplified, as it no longer needs to update button visibility.

With these changes, the visibility of the scroll buttons is now determined by the converter based on the scrollable state of the `TabScrollViewer`. This approach provides a cleaner and more maintainable solution.
# Q4: The binding of the visibillty only depends on the tabstripplacement
Alright, if the visibility of the scroll buttons should only depend on the `TabStripPlacement`, you can simplify the converter and binding. Here’s how you can modify the code:

First, create a simplified converter:

```csharp
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WpfDockingManager
{
    public class TabStripPlacementVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Dock tabStripPlacement)
            {
                string direction = parameter as string;

                if (direction == "Top" || direction == "Bottom")
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
```

Next, update your XAML to use this converter:

```xml
<TabControl
    x:Class="WpfDockingManager.DragTabControl"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:local="clr-namespace:WpfDockingManager"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    x:Name="dragTabControl"
    AllowDrop="True"
    d:DesignHeight="450"
    d:DesignWidth="800"
    DragEnter="OnDragEnter"
    DragOver="OnDragOver"
    Drop="OnDrop"
    mc:Ignorable="d"
    PreviewMouseLeftButtonDown="OnPreviewMouseLeftButtonDown"
    PreviewMouseMove="OnPreviewMouseMove">
    <TabControl.Resources>
        <local:TabItemRotation x:Key="TabItemRotation" />
        <local:DockOrientationConverter x:Key="DockOrientationConverter" />
        <local:DockOrientationVisibillityConverter x:Key="DockOrientationVisibillity" />
        <local:GridRowAdjustmentConverter x:Key="GridRowAdjustment" />
        <local:TabStripPlacementVisibilityConverter x:Key="TabStripPlacementVisibilityConverter" />

        <Style x:Key="CloseButtonStyle" TargetType="Button">
            <Setter Property="Background" Value="Transparent" />
            <Setter Property="BorderThickness" Value="0" />
            <Setter Property="HorizontalAlignment" Value="Center" />
            <Setter Property="VerticalAlignment" Value="Center" />
            <Setter Property="Width" Value="16" />
            <Setter Property="Height" Value="16" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="Button">
                        <Grid>
                            <Path
                                x:Name="Cross"
                                Data="M4,4 L13,13 M4,13 L13,4"
                                Stroke="Gray"
                                StrokeThickness="2" />
                        </Grid>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver" Value="True">
                                <Setter TargetName="Cross" Property="Stroke" Value="Black" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
            <EventSetter Event="Button.Click" Handler="OnCloseButtonEvent" />
        </Style>

        <Style TargetType="{x:Type TabItem}">
            <Setter Property="HeaderTemplate">
                <Setter.Value>
                    <DataTemplate>
                        <ContentPresenter Content="{TemplateBinding Content}">
                            <ContentPresenter.LayoutTransform>
                                <RotateTransform
                                    Angle="{Binding ElementName=dragTabControl, Converter={StaticResource TabItemRotation}, Path=TabStripPlacement}" />
                            </ContentPresenter.LayoutTransform>
                        </ContentPresenter>
                    </DataTemplate>
                </Setter.Value>
            </Setter>
            <Setter Property="Padding" Value="3" />
        </Style>
    </TabControl.Resources>

    <!--  Modified Template  -->
    <TabControl.Template>
        <ControlTemplate TargetType="TabControl">
            <Grid>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto" />
                    <RowDefinition Height="*" />
                </Grid.RowDefinitions>

                <!--  Custom TabPanel with Scroll Buttons  -->
                <Border
                    Background="{TemplateBinding Background}"
                    BorderBrush="{TemplateBinding BorderBrush}"
                    BorderThickness="{TemplateBinding BorderThickness}">
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="Auto" />
                            <ColumnDefinition Width="*" />
                            <ColumnDefinition Width="Auto" />
                        </Grid.ColumnDefinitions>

                        <!--  Left Scroll Button  -->
                        <Button
                            x:Name="LeftScrollButton"
                            Click="LeftScrollButton_Click"
                            Content="&lt;"
                            FontSize="16"
                            FontWeight="Bold"
                            Margin="2"
                            Padding="5,0"
                            Visibility="{Binding RelativeSource={RelativeSource AncestorTemplate}, Path=TemplatedParent.TabStripPlacement, Converter={StaticResource TabStripPlacementVisibilityConverter}, ConverterParameter='Left'}">
                        </Button>

                        <!--  ScrollViewer for Tabs  -->
                        <ScrollViewer
                            x:Name="TabScrollViewer"
                            Grid.Column="1"
                            HorizontalScrollBarVisibility="Hidden"
                            VerticalScrollBarVisibility="Hidden"
                            ScrollChanged="TabScrollViewer_ScrollChanged">
                            <TabPanel
                                x:Name="TabPanel"
                                Background="Transparent"
                                IsItemsHost="True"
                                Margin="0" />
                        </ScrollViewer>

                        <!--  Right Scroll Button  -->
                        <Button
                            x:Name="RightScrollButton"
                            Grid.Column="2"
                            Click="RightScrollButton_Click"
                            Content=">"
                            FontSize="16"
                            FontWeight="Bold"
                            Margin="2"
                            Padding="5,0"
                            Visibility="{Binding RelativeSource={RelativeSource AncestorTemplate}, Path=TemplatedParent.TabStripPlacement, Converter={StaticResource TabStripPlacementVisibilityConverter}, ConverterParameter='Right'}">
                        </Button>
                    </Grid>
                </Border>

                <!--  ContentPresenter  -->
                <ContentPresenter
                    Grid.Row="1"
                    ContentSource="SelectedContent" />
            </Grid>
        </ControlTemplate>
    </TabControl.Template>

    <TabControl.ItemContainerStyle>
        <Style BasedOn="{StaticResource {x:Type TabItem}}" TargetType="TabItem">
            <Style.Triggers>
                <DataTrigger
                    Binding="{Binding RelativeSource={RelativeSource Self}, Path=TabStripPlacement, Converter={StaticResource ResourceKey=DockOrientationConverter}}"
                    Value="Horizontal">
                    <Setter
                        Property="Height"
                        Value="{Binding TabHeight, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" />
                    <Setter Property="Template">
                        <Setter.Value>
                            <ControlTemplate TargetType="TabItem">
                                <Grid>
                                    <Border
                                        x:Name="Border"
                                        Background="{TemplateBinding Background}"
                                        BorderBrush="{TemplateBinding BorderBrush}"
                                        BorderThickness="{TemplateBinding BorderThickness}"
                                        CornerRadius="2"
                                        Margin="2,2,2,0">
                                        <Grid Name="_btnGrid">
                                            <Grid.ColumnDefinitions>
                                                <ColumnDefinition Width="*" />
                                                <ColumnDefinition Width="Auto" />
                                            </Grid.ColumnDefinitions>

                                            <ContentPresenter
                                                x:Name="ContentSite"
                                                Grid.Column="0"
                                                ContentSource="Header"
                                                HorizontalAlignment="Center"
                                                Margin="5,2,5,2"
                                                RecognizesAccessKey="True"
                                                VerticalAlignment="Center" />

                                            <Button
                                                x:Name="CloseButton"
                                                Grid.Column="1"
                                                Command="{Binding CloseTabCommand, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}"
                                                CommandParameter="{Binding}"
                                                Margin="5,0,5,0"
                                                Style="{StaticResource CloseButtonStyle}" />
                                        </Grid>
                                    </Border>
                                </Grid>
                                <ControlTemplate.Triggers>
                                    <Trigger Property="IsSelected" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="{DynamicResource {x:Static SystemColors.WindowBrushKey}}" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="DarkGray" />
                                    </Trigger>
                                    <Trigger Property="IsSelected" Value="False">
                                        <Setter TargetName="Border" Property="Background" Value="LightGray" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="Gray" />
                                    </Trigger>
                                    <Trigger Property="IsMouseOver" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="#FFDDDDDD" />
                                    </Trigger>
                                </ControlTemplate.Triggers>
                            </ControlTemplate>
                        </Setter.Value>
                    </Setter>
                </DataTrigger>
                <DataTrigger
                    Binding="{Binding RelativeSource={RelativeSource Self}, Path=TabStripPlacement, Converter={StaticResource ResourceKey=DockOrientationConverter}}"
                    Value="Vertical">
                    <Setter
                        Property="Width"
                        Value="{Binding TabHeight, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" />
                    <Setter Property="Template">
                        <Setter.Value>
                            <ControlTemplate TargetType="TabItem">
                                <Grid>
                                    <Border
                                        x:Name="Border"
                                        Background="{TemplateBinding Background}"
                                        BorderBrush="{TemplateBinding BorderBrush}"
                                        BorderThickness="{TemplateBinding BorderThickness}"
                                        CornerRadius="2"
                                        Margin="2,2,2,0">
                                        <Grid Name="_btnGrid">
                                            <Grid.RowDefinitions>
                                                <RowDefinition Height="Auto" />
                                                <RowDefinition Height="*" />
                                            </Grid.RowDefinitions>

                                            <ContentPresenter
                                                x:Name="ContentSite"
                                                Grid.Row="{Binding 
                                                    RelativeSource={RelativeSource TemplatedParent}, 
                                                    Path=TabStripPlacement, 
                                                    Converter={StaticResource ResourceKey=GridRowAdjustment}, 
                                                    ConverterParameter=content}"
                                                ContentSource="Header"
                                                HorizontalAlignment="Center"
                                                Margin="5,2,5,2"
                                                RecognizesAccessKey="True"
                                                VerticalAlignment="Center" />

                                            <Button
                                                x:Name="CloseButton"
                                                Grid.Row="{Binding 
                                                    RelativeSource={RelativeSource TemplatedParent}, 
                                                    Path=TabStripPlacement, 
                                                    Converter={StaticResource ResourceKey=GridRowAdjustment}, 
                                                    ConverterParameter=button}"
                                                Command="{Binding CloseTabCommand, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}"
                                                CommandParameter="{Binding}"
                                                Margin="5,0,5,0"
                                                Style="{StaticResource CloseButtonStyle}" />
                                        </Grid>
                                    </Border>
                                </Grid>
                                <ControlTemplate.Triggers>
                                    <Trigger Property="IsSelected" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="{DynamicResource {x:Static SystemColors.WindowBrushKey}}" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="DarkGray" />
                                    </Trigger>
                                    <Trigger Property="IsSelected" Value="False">
                                        <Setter TargetName="Border" Property="Background" Value="LightGray" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="Gray" />
                                    </Trigger>
                                    <Trigger Property="IsMouseOver" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="#FFDDDDDD" />
                                    </Trigger>
                                </ControlTemplate.Triggers>
                            </ControlTemplate>
                        </Setter.Value>
                    </Setter>
                </DataTrigger>
            </Style.Triggers>
            <Setter
                Property="Width"
                Value="{Binding TabWidth, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" />
        </Style>
    </TabControl.ItemContainerStyle>

</TabControl>
```

```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfDockingManager
{
    public partial class DragTabControl : TabControl
    {
        public DragTabControl()
        {
            InitializeComponent();
        }

        private void LeftScrollButton_Click(object sender, RoutedEventArgs e)
        {
            TabScrollViewer.ScrollToHorizontalOffset(TabScrollViewer.HorizontalOffset - 50);
        }

        private void RightScrollButton_Click(object sender, RoutedEventArgs e)
        {
            TabScrollViewer.ScrollToHorizontalOffset(TabScrollViewer.HorizontalOffset + 50);
        }

        private void TabScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Your existing logic
        }

        private void OnPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Your existing logic
        }

        private void OnPreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Your existing logic
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            // Your existing logic
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            // Your existing logic
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            // Your existing logic
        }

        private void OnCloseButtonEvent(object sender, RoutedEventArgs e)
        {
            // Your existing logic
        }
    }
}
```

Key changes and explanations:

1.  **TabStripPlacementVisibilityConverter**:
    *   This `IValueConverter` takes the `TabStripPlacement` as input.
    *   It checks if the `TabStripPlacement` is `Top` or `Bottom`.
    *   It returns `Visibility.Visible` if the `TabStripPlacement` is `Top` or `Bottom`, and `Visibility.Collapsed` otherwise.
2.  **XAML Binding**:
    *   The `Visibility` property of both `LeftScrollButton` and `RightScrollButton` is bound to the `TabStripPlacement` property of the `TabControl`.
    *   The `Converter` is set to `TabStripPlacementVisibilityConverter`.
    *   A `ConverterParameter` is passed to the converter, but it's not actually used in this implementation. You can remove it for clarity.
3.  **Code-Behind**:
    *   The code-behind remains the same as in the previous example.

With these changes, the visibility of the scroll buttons is now solely determined by the `TabStripPlacement` property of the `TabControl`. If the `TabStripPlacement` is `Top` or `Bottom`, the buttons are visible; otherwise, they are collapsed.
# Q5: If tabstripplacement is left or right, then the buttons must be placed at the top and bottom of the header and scroll accordingly up and down
Please login to search for FREE!
# Q6: if tabstripplacement is left or right, then the buttons must be placed at the top and bottom of the header and scroll accordingly
Please login to search for FREE!
# Q7: If tabstripplacement is left or right, then the buttons must be placed at the top and bottom of the header and scroll accordingly.
Please login to search for FREE!
# Q8: If tabstripplacement is left or right, then the buttons must be placed at the top and bottom of the header and scroll accordingly.
Understood. To place the scroll buttons at the top and bottom of the header when the `TabStripPlacement` is `Left` or `Right`, and make them scroll vertically, you'll need to modify the `TabControl`'s template and adjust the scrolling logic. Here’s how you can do it:

First, update the XAML for the `TabControl` template:

```xml
<TabControl
    x:Class="WpfDockingManager.DragTabControl"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:local="clr-namespace:WpfDockingManager"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    x:Name="dragTabControl"
    AllowDrop="True"
    d:DesignHeight="450"
    d:DesignWidth="800"
    DragEnter="OnDragEnter"
    DragOver="OnDragOver"
    Drop="OnDrop"
    mc:Ignorable="d"
    PreviewMouseLeftButtonDown="OnPreviewMouseLeftButtonDown"
    PreviewMouseMove="OnPreviewMouseMove">
    <TabControl.Resources>
        <local:TabItemRotation x:Key="TabItemRotation" />
        <local:DockOrientationConverter x:Key="DockOrientationConverter" />
        <local:DockOrientationVisibillityConverter x:Key="DockOrientationVisibillity" />
        <local:GridRowAdjustmentConverter x:Key="GridRowAdjustment" />
        <local:TabStripPlacementVisibilityConverter x:Key="TabStripPlacementVisibilityConverter" />

        <Style x:Key="CloseButtonStyle" TargetType="Button">
            <Setter Property="Background" Value="Transparent" />
            <Setter Property="BorderThickness" Value="0" />
            <Setter Property="HorizontalAlignment" Value="Center" />
            <Setter Property="VerticalAlignment" Value="Center" />
            <Setter Property="Width" Value="16" />
            <Setter Property="Height" Value="16" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="Button">
                        <Grid>
                            <Path
                                x:Name="Cross"
                                Data="M4,4 L13,13 M4,13 L13,4"
                                Stroke="Gray"
                                StrokeThickness="2" />
                        </Grid>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver" Value="True">
                                <Setter TargetName="Cross" Property="Stroke" Value="Black" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
            <EventSetter Event="Button.Click" Handler="OnCloseButtonEvent" />
        </Style>

        <Style TargetType="{x:Type TabItem}">
            <Setter Property="HeaderTemplate">
                <Setter.Value>
                    <DataTemplate>
                        <ContentPresenter Content="{TemplateBinding Content}">
                            <ContentPresenter.LayoutTransform>
                                <RotateTransform
                                    Angle="{Binding ElementName=dragTabControl, Converter={StaticResource TabItemRotation}, Path=TabStripPlacement}" />
                            </ContentPresenter.LayoutTransform>
                        </ContentPresenter>
                    </DataTemplate>
                </Setter.Value>
            </Setter>
            <Setter Property="Padding" Value="3" />
        </Style>
    </TabControl.Resources>

    <!--  Modified Template  -->
    <TabControl.Template>
        <ControlTemplate TargetType="TabControl">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="Auto" />
                    <ColumnDefinition Width="*" />
                </Grid.ColumnDefinitions>
                <Grid.RowDefinitions>
                    <RowDefinition Height="*" />
                </Grid.RowDefinitions>

                <!--  Vertical Scroll Buttons and TabPanel  -->
                <Border
                    Grid.Column="0"
                    Background="{TemplateBinding Background}"
                    BorderBrush="{TemplateBinding BorderBrush}"
                    BorderThickness="{TemplateBinding BorderThickness}"
                    Visibility="{Binding TabStripPlacement, RelativeSource={RelativeSource TemplatedParent}, Converter={StaticResource DockOrientationVisibillity}, ConverterParameter=Vertical}">
                    <Grid>
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto" />
                            <RowDefinition Height="*" />
                            <RowDefinition Height="Auto" />
                        </Grid.RowDefinitions>

                        <!--  Top Scroll Button  -->
                        <Button
                            x:Name="TopScrollButton"
                            Click="TopScrollButton_Click"
                            Content="^"
                            FontSize="16"
                            FontWeight="Bold"
                            Margin="2"
                            Padding="5,0">
                            <Button.Visibility>
                                <Binding
                                    Converter="{StaticResource TabStripPlacementVisibilityConverter}"
                                    Path="TabStripPlacement"
                                    RelativeSource="{RelativeSource TemplatedParent}" />
                            </Button.Visibility>
                        </Button>

                        <!--  ScrollViewer for Tabs  -->
                        <ScrollViewer
                            x:Name="TabScrollViewer"
                            Grid.Row="1"
                            HorizontalScrollBarVisibility="Hidden"
                            VerticalScrollBarVisibility="Hidden"
                            ScrollChanged="TabScrollViewer_ScrollChanged">
                            <TabPanel
                                x:Name="TabPanel"
                                Background="Transparent"
                                IsItemsHost="True"
                                Margin="0" />
                        </ScrollViewer>

                        <!--  Bottom Scroll Button  -->
                        <Button
                            x:Name="BottomScrollButton"
                            Grid.Row="2"
                            Click="BottomScrollButton_Click"
                            Content="v"
                            FontSize="16"
                            FontWeight="Bold"
                            Margin="2"
                            Padding="5,0">
                            <Button.Visibility>
                                <Binding
                                    Converter="{StaticResource TabStripPlacementVisibilityConverter}"
                                    Path="TabStripPlacement"
                                    RelativeSource="{RelativeSource TemplatedParent}" />
                            </Button.Visibility>
                        </Button>
                    </Grid>
                </Border>

                <!--  Horizontal Scroll Buttons and TabPanel  -->
                <Border
                    Grid.Column="0"
                    Background="{TemplateBinding Background}"
                    BorderBrush="{TemplateBinding BorderBrush}"
                    BorderThickness="{TemplateBinding BorderThickness}"
                    Visibility="{Binding TabStripPlacement, RelativeSource={RelativeSource TemplatedParent}, Converter={StaticResource DockOrientationVisibillity}, ConverterParameter=Horizontal}">
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="Auto" />
                            <ColumnDefinition Width="*" />
                            <ColumnDefinition Width="Auto" />
                        </Grid.ColumnDefinitions>

                        <!--  Left Scroll Button  -->
                        <Button
                            x:Name="LeftScrollButton"
                            Click="LeftScrollButton_Click"
                            Content="&lt;"
                            FontSize="16"
                            FontWeight="Bold"
                            Margin="2"
                            Padding="5,0">
                            <Button.Visibility>
                                <Binding
                                    Converter="{StaticResource TabStripPlacementVisibilityConverter}"
                                    Path="TabStripPlacement"
                                    RelativeSource="{RelativeSource TemplatedParent}" />
                            </Button.Visibility>
                        </Button>

                        <!--  ScrollViewer for Tabs  -->
                        <ScrollViewer
                            x:Name="TabScrollViewer"
                            Grid.Column="1"
                            HorizontalScrollBarVisibility="Hidden"
                            VerticalScrollBarVisibility="Hidden"
                            ScrollChanged="TabScrollViewer_ScrollChanged">
                            <TabPanel
                                x:Name="TabPanel"
                                Background="Transparent"
                                IsItemsHost="True"
                                Margin="0" />
                        </ScrollViewer>

                        <!--  Right Scroll Button  -->
                        <Button
                            x:Name="RightScrollButton"
                            Grid.Column="2"
                            Click="RightScrollButton_Click"
                            Content=">"
                            FontSize="16"
                            FontWeight="Bold"
                            Margin="2"
                            Padding="5,0">
                            <Button.Visibility>
                                <Binding
                                    Converter="{StaticResource TabStripPlacementVisibilityConverter}"
                                    Path="TabStripPlacement"
                                    RelativeSource="{RelativeSource TemplatedParent}" />
                            </Button.Visibility>
                        </Button>
                    </Grid>
                </Border>

                <!--  ContentPresenter  -->
                <ContentPresenter
                    Grid.Column="1"
                    ContentSource="SelectedContent" />
            </Grid>
        </ControlTemplate>
    </TabControl.Template>

    <TabControl.ItemContainerStyle>
        <Style BasedOn="{StaticResource {x:Type TabItem}}" TargetType="TabItem">
            <Style.Triggers>
                <DataTrigger
                    Binding="{Binding RelativeSource={RelativeSource Self}, Path=TabStripPlacement, Converter={StaticResource ResourceKey=DockOrientationConverter}}"
                    Value="Horizontal">
                    <Setter
                        Property="Height"
                        Value="{Binding TabHeight, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" />
                    <Setter Property="Template">
                        <Setter.Value>
                            <ControlTemplate TargetType="TabItem">
                                <Grid>
                                    <Border
                                        x:Name="Border"
                                        Background="{TemplateBinding Background}"
                                        BorderBrush="{TemplateBinding BorderBrush}"
                                        BorderThickness="{TemplateBinding BorderThickness}"
                                        CornerRadius="2"
                                        Margin="2,2,2,0">
                                        <Grid Name="_btnGrid">
                                            <Grid.ColumnDefinitions>
                                                <ColumnDefinition Width="*" />
                                                <ColumnDefinition Width="Auto" />
                                            </Grid.ColumnDefinitions>

                                            <ContentPresenter
                                                x:Name="ContentSite"
                                                Grid.Column="0"
                                                ContentSource="Header"
                                                HorizontalAlignment="Center"
                                                Margin="5,2,5,2"
                                                RecognizesAccessKey="True"
                                                VerticalAlignment="Center" />

                                            <Button
                                                x:Name="CloseButton"
                                                Grid.Column="1"
                                                Command="{Binding CloseTabCommand, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}"
                                                CommandParameter="{Binding}"
                                                Margin="5,0,5,0"
                                                Style="{StaticResource CloseButtonStyle}" />
                                        </Grid>
                                    </Border>
                                </Grid>
                                <ControlTemplate.Triggers>
                                    <Trigger Property="IsSelected" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="{DynamicResource {x:Static SystemColors.WindowBrushKey}}" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="DarkGray" />
                                    </Trigger>
                                    <Trigger Property="IsSelected" Value="False">
                                        <Setter TargetName="Border" Property="Background" Value="LightGray" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="Gray" />
                                    </Trigger>
                                    <Trigger Property="IsMouseOver" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="#FFDDDDDD" />
                                    </Trigger>
                                </ControlTemplate.Triggers>
                            </ControlTemplate>
                        </Setter.Value>
                    </Setter>
                </DataTrigger>
                <DataTrigger
                    Binding="{Binding RelativeSource={RelativeSource Self}, Path=TabStripPlacement, Converter={StaticResource ResourceKey=DockOrientationConverter}}"
                    Value="Vertical">
                    <Setter
                        Property="Width"
                        Value="{Binding TabHeight, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" />
                    <Setter Property="Template">
                        <Setter.Value>
                            <ControlTemplate TargetType="TabItem">
                                <Grid>
                                    <Border
                                        x:Name="Border"
                                        Background="{TemplateBinding Background}"
                                        BorderBrush="{TemplateBinding BorderBrush}"
                                        BorderThickness="{TemplateBinding BorderThickness}"
                                        CornerRadius="2"
                                        Margin="2,2,2,0">
                                        <Grid Name="_btnGrid">
                                            <Grid.RowDefinitions>
                                                <RowDefinition Height="Auto" />
                                                <RowDefinition Height="*" />
                                            </Grid.RowDefinitions>

                                            <ContentPresenter
                                                x:Name="ContentSite"
                                                Grid.Row="{Binding 
                                                    RelativeSource={RelativeSource TemplatedParent}, 
                                                    Path=TabStripPlacement, 
                                                    Converter={StaticResource ResourceKey=GridRowAdjustment}, 
                                                    ConverterParameter=content}"
                                                ContentSource="Header"
                                                HorizontalAlignment="Center"
                                                Margin="5,2,5,2"
                                                RecognizesAccessKey="True"
                                                VerticalAlignment="Center" />

                                            <Button
                                                x:Name="CloseButton"
                                                Grid.Row="{Binding 
                                                    RelativeSource={RelativeSource TemplatedParent}, 
                                                    Path=TabStripPlacement, 
                                                    Converter={StaticResource ResourceKey=GridRowAdjustment}, 
                                                    ConverterParameter=button}"
                                                Command="{Binding CloseTabCommand, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}"
                                                CommandParameter="{Binding}"
                                                Margin="5,0,5,0"
                                                Style="{StaticResource CloseButtonStyle}" />
                                        </Grid>
                                    </Border>
                                </Grid>
                                <ControlTemplate.Triggers>
                                    <Trigger Property="IsSelected" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="{DynamicResource {x:Static SystemColors.WindowBrushKey}}" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="DarkGray" />
                                    </Trigger>
                                    <Trigger Property="IsSelected" Value="False">
                                        <Setter TargetName="Border" Property="Background" Value="LightGray" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="Gray" />
                                    </Trigger>
                                    <Trigger Property="IsMouseOver" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="#FFDDDDDD" />
                                    </Trigger>
                                </ControlTemplate.Triggers>
                            </ControlTemplate>
                        </Setter.Value>
                    </Setter>
                </DataTrigger>
            </Style.Triggers>
            <Setter
                Property="Width"
                Value="{Binding TabWidth, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" />
        </Style>
    </TabControl.ItemContainerStyle>

</TabControl>
```

```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WpfDockingManager
{
    public partial class DragTabControl : TabControl
    {
        public DragTabControl()
        {
            InitializeComponent();
        }

        private void LeftScrollButton_Click(object sender, RoutedEventArgs e)
        {
            TabScrollViewer.ScrollToHorizontalOffset(TabScrollViewer.HorizontalOffset - 50);
        }

        private void RightScrollButton_Click(object sender, RoutedEventArgs e)
        {
            TabScrollViewer.ScrollToHorizontalOffset(TabScrollViewer.HorizontalOffset + 50);
        }

        private void TopScrollButton_Click(object sender, RoutedEventArgs e)
        {
            TabScrollViewer.ScrollToVerticalOffset(TabScrollViewer.VerticalOffset - 50);
        }

        private void BottomScrollButton_Click(object sender, RoutedEventArgs e)
        {
            TabScrollViewer.ScrollToVerticalOffset(TabScrollViewer.VerticalOffset + 50);
        }

        private void TabScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Your existing logic
        }

        private void OnPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Your existing logic
        }

        private void OnPreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Your existing logic
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            // Your existing logic
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            // Your existing logic
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            // Your existing logic
        }

        private void OnCloseButtonEvent(object sender, RoutedEventArgs e)
        {
            // Your existing logic
        }
    }
}
```

```csharp
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WpfDockingManager
{
    public class TabStripPlacementVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Dock tabStripPlacement)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
```

Key changes and explanations:

1.  **Template Modification**:
    *   The `TabControl` template is updated to include separate grids for horizontal (Top/Bottom) and vertical (Left/Right) `TabStripPlacement`.
    *   For vertical `TabStripPlacement`, `TopScrollButton` and `BottomScrollButton` are added at the top and bottom of the `TabPanel`.
    *   For horizontal `TabStripPlacement`, `LeftScrollButton` and `RightScrollButton` are added to the left and right of the `TabPanel`.
    *   The visibility of these sections is controlled by the `DockOrientationVisibillityConverter`.
2.  **DockOrientationVisibillityConverter**:
    *   This converter checks if the `TabStripPlacement` is horizontal (Top/Bottom) or vertical (Left/Right) and returns the appropriate `Visibility` value.
3.  **Scroll Buttons**:
    *   `TopScrollButton` and `BottomScrollButton` are added with click event handlers.
    *   The content of the buttons is set to "^" and "v" for visual indication.
4.  **ScrollViewer**:
    *   The `ScrollViewer` is configured to hide both horizontal and vertical scrollbars.
5.  **Event Handlers**:
    *   `TopScrollButton_Click` and `BottomScrollButton_Click` scroll the `TabScrollViewer` vertically.
    *   `LeftScrollButton_Click` and `RightScrollButton_Click` scroll the `TabScrollViewer` horizontally.
6.  **TabStripPlacementVisibilityConverter**:
    *   This converter is simplified to always return `Visibility.Visible` when the `TabStripPlacement` is valid.

With these changes, the scroll buttons will be placed at the top and bottom for vertical `TabStripPlacement` and will scroll vertically. For horizontal `TabStripPlacement`, the buttons will be placed on the left and right and scroll horizontally.
# Q9: Now the left right is missing. It would need a trigger on the template to handle either left/right vs. up/down depending on the orientation of the tabstriplacement
You're right, we need to use triggers to dynamically switch between horizontal and vertical layouts based on the `TabStripPlacement`. This involves modifying the `TabControl` template to include these triggers.

Here’s the updated XAML:

```xml
<TabControl
    x:Class="WpfDockingManager.DragTabControl"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:local="clr-namespace:WpfDockingManager"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    x:Name="dragTabControl"
    AllowDrop="True"
    d:DesignHeight="450"
    d:DesignWidth="800"
    DragEnter="OnDragEnter"
    DragOver="OnDragOver"
    Drop="OnDrop"
    mc:Ignorable="d"
    PreviewMouseLeftButtonDown="OnPreviewMouseLeftButtonDown"
    PreviewMouseMove="OnPreviewMouseMove">
    <TabControl.Resources>
        <local:TabItemRotation x:Key="TabItemRotation" />
        <local:DockOrientationConverter x:Key="DockOrientationConverter" />
        <local:DockOrientationVisibillityConverter x:Key="DockOrientationVisibillity" />
        <local:GridRowAdjustmentConverter x:Key="GridRowAdjustment" />
        <local:TabStripPlacementVisibilityConverter x:Key="TabStripPlacementVisibilityConverter" />

        <Style x:Key="CloseButtonStyle" TargetType="Button">
            <Setter Property="Background" Value="Transparent" />
            <Setter Property="BorderThickness" Value="0" />
            <Setter Property="HorizontalAlignment" Value="Center" />
            <Setter Property="VerticalAlignment" Value="Center" />
            <Setter Property="Width" Value="16" />
            <Setter Property="Height" Value="16" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="Button">
                        <Grid>
                            <Path
                                x:Name="Cross"
                                Data="M4,4 L13,13 M4,13 L13,4"
                                Stroke="Gray"
                                StrokeThickness="2" />
                        </Grid>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver" Value="True">
                                <Setter TargetName="Cross" Property="Stroke" Value="Black" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
            <EventSetter Event="Button.Click" Handler="OnCloseButtonEvent" />
        </Style>

        <Style TargetType="{x:Type TabItem}">
            <Setter Property="HeaderTemplate">
                <Setter.Value>
                    <DataTemplate>
                        <ContentPresenter Content="{TemplateBinding Content}">
                            <ContentPresenter.LayoutTransform>
                                <RotateTransform
                                    Angle="{Binding ElementName=dragTabControl, Converter={StaticResource TabItemRotation}, Path=TabStripPlacement}" />
                            </ContentPresenter.LayoutTransform>
                        </ContentPresenter>
                    </DataTemplate>
                </Setter.Value>
            </Setter>
            <Setter Property="Padding" Value="3" />
        </Style>
    </TabControl.Resources>

    <!--  Modified Template  -->
    <TabControl.Template>
        <ControlTemplate TargetType="TabControl">
            <Grid>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto" />
                    <RowDefinition Height="*" />
                </Grid.RowDefinitions>

                <Border
                    Background="{TemplateBinding Background}"
                    BorderBrush="{TemplateBinding BorderBrush}"
                    BorderThickness="{TemplateBinding BorderThickness}">
                    <Grid x:Name="MainGrid">
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto" />
                            <RowDefinition Height="*" />
                        </Grid.RowDefinitions>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="Auto" />
                            <ColumnDefinition Width="*" />
                            <ColumnDefinition Width="Auto" />
                        </Grid.ColumnDefinitions>

                        <!--  ScrollViewer for Tabs  -->
                        <ScrollViewer
                            x:Name="TabScrollViewer"
                            Grid.Row="0"
                            Grid.Column="1"
                            HorizontalScrollBarVisibility="Hidden"
                            VerticalScrollBarVisibility="Hidden"
                            ScrollChanged="TabScrollViewer_ScrollChanged">
                            <TabPanel
                                x:Name="TabPanel"
                                Background="Transparent"
                                IsItemsHost="True"
                                Margin="0" />
                        </ScrollViewer>

                        <!--  Left Scroll Button  -->
                        <Button
                            x:Name="LeftScrollButton"
                            Grid.Row="0"
                            Grid.Column="0"
                            Click="LeftScrollButton_Click"
                            Content="&lt;"
                            FontSize="16"
                            FontWeight="Bold"
                            Margin="2"
                            Padding="5,0"
                            Visibility="{Binding TabStripPlacement, RelativeSource={RelativeSource AncestorTemplate}, Converter={StaticResource DockOrientationVisibillity}, ConverterParameter=Horizontal}">
                        </Button>

                        <!--  Right Scroll Button  -->
                        <Button
                            x:Name="RightScrollButton"
                            Grid.Row="0"
                            Grid.Column="2"
                            Click="RightScrollButton_Click"
                            Content=">"
                            FontSize="16"
                            FontWeight="Bold"
                            Margin="2"
                            Padding="5,0"
                            Visibility="{Binding TabStripPlacement, RelativeSource={RelativeSource AncestorTemplate}, Converter={StaticResource DockOrientationVisibillity}, ConverterParameter=Horizontal}">
                        </Button>

                        <!--  Top Scroll Button  -->
                        <Button
                            x:Name="TopScrollButton"
                            Grid.Row="0"
                            Grid.Column="0"
                            Click="TopScrollButton_Click"
                            Content="^"
                            FontSize="16"
                            FontWeight="Bold"
                            Margin="2"
                            Padding="5,0"
                            Visibility="Collapsed">
                        </Button>

                        <!--  Bottom Scroll Button  -->
                        <Button
                            x:Name="BottomScrollButton"
                            Grid.Row="2"
                            Grid.Column="0"
                            Click="BottomScrollButton_Click"
                            Content="v"
                            FontSize="16"
                            FontWeight="Bold"
                            Margin="2"
                            Padding="5,0"
                            Visibility="Collapsed">
                        </Button>

                        <ContentPresenter
                            Grid.Row="1"
                            Grid.Column="0"
                            Grid.ColumnSpan="3"
                            ContentSource="SelectedContent" />
                    </Grid>
                </Border>
                <ControlTemplate.Triggers>
                    <Trigger Property="TabStripPlacement" Value="Left">
                        <Setter TargetName="MainGrid" Property="Grid.ColumnDefinitions">
                            <Setter.Value>
                                <ColumnDefinitionCollection>
                                    <ColumnDefinition Width="Auto" />
                                    <ColumnDefinition Width="*" />
                                </ColumnDefinitionCollection>
                            </Setter.Value>
                        </Setter>
                        <Setter TargetName="MainGrid" Property="Grid.RowDefinitions">
                            <Setter.Value>
                                <RowDefinitionCollection>
                                    <RowDefinition Height="Auto" />
                                    <RowDefinition Height="*" />
                                    <RowDefinition Height="Auto" />
                                </RowDefinitionCollection>
                            </Setter.Value>
                        </Setter>
                        <Setter TargetName="TabScrollViewer" Property="Grid.Row" Value="1" />
                        <Setter TargetName="TabScrollViewer" Property="Grid.Column" Value="0" />
                        <Setter TargetName="LeftScrollButton" Property="Visibility" Value="Collapsed" />
                        <Setter TargetName="RightScrollButton" Property="Visibility" Value="Collapsed" />
                        <Setter TargetName="TopScrollButton" Property="Visibility" Value="Visible" />
                        <Setter TargetName="BottomScrollButton" Property="Visibility" Value="Visible" />
                    </Trigger>
                    <Trigger Property="TabStripPlacement" Value="Right">
                        <Setter TargetName="MainGrid" Property="Grid.ColumnDefinitions">
                            <Setter.Value>
                                <ColumnDefinitionCollection>
                                    <ColumnDefinition Width="Auto" />
                                    <ColumnDefinition Width="*" />
                                </ColumnDefinitionCollection>
                            </Setter.Value>
                        </Setter>
                        <Setter TargetName="MainGrid" Property="Grid.RowDefinitions">
                            <Setter.Value>
                                <RowDefinitionCollection>
                                    <RowDefinition Height="Auto" />
                                    <RowDefinition Height="*" />
                                    <RowDefinition Height="Auto" />
                                </RowDefinitionCollection>
                            </Setter.Value>
                        </Setter>
                        <Setter TargetName="TabScrollViewer" Property="Grid.Row" Value="1" />
                        <Setter TargetName="TabScrollViewer" Property="Grid.Column" Value="0" />
                        <Setter TargetName="LeftScrollButton" Property="Visibility" Value="Collapsed" />
                        <Setter TargetName="RightScrollButton" Property="Visibility" Value="Collapsed" />
                        <Setter TargetName="TopScrollButton" Property="Visibility" Value="Visible" />
                        <Setter TargetName="BottomScrollButton" Property="Visibility" Value="Visible" />
                    </Trigger>
                    <Trigger Property="TabStripPlacement" Value="Top">
                        <Setter TargetName="TopScrollButton" Property="Visibility" Value="Collapsed" />
                        <Setter TargetName="BottomScrollButton" Property="Visibility" Value="Collapsed" />
                        <Setter TargetName="LeftScrollButton" Property="Visibility" Value="Visible" />
                        <Setter TargetName="RightScrollButton" Property="Visibility" Value="Visible" />
                    </Trigger>
                    <Trigger Property="TabStripPlacement" Value="Bottom">
                        <Setter TargetName="TopScrollButton" Property="Visibility" Value="Collapsed" />
                        <Setter TargetName="BottomScrollButton" Property="Visibility" Value="Collapsed" />
                        <Setter TargetName="LeftScrollButton" Property="Visibility" Value="Visible" />
                        <Setter TargetName="RightScrollButton" Property="Visibility" Value="Visible" />
                    </Trigger>
                </ControlTemplate.Triggers>
            </Grid>
        </ControlTemplate>
    </TabControl.Template>

    <TabControl.ItemContainerStyle>
        <Style BasedOn="{StaticResource {x:Type TabItem}}" TargetType="TabItem">
            <Style.Triggers>
                <DataTrigger
                    Binding="{Binding RelativeSource={RelativeSource Self}, Path=TabStripPlacement, Converter={StaticResource ResourceKey=DockOrientationConverter}}"
                    Value="Horizontal">
                    <Setter
                        Property="Height"
                        Value="{Binding TabHeight, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" />
                    <Setter Property="Template">
                        <Setter.Value>
                            <ControlTemplate TargetType="TabItem">
                                <Grid>
                                    <Border
                                        x:Name="Border"
                                        Background="{TemplateBinding Background}"
                                        BorderBrush="{TemplateBinding BorderBrush}"
                                        BorderThickness="{TemplateBinding BorderThickness}"
                                        CornerRadius="2"
                                        Margin="2,2,2,0">
                                        <Grid Name="_btnGrid">
                                            <Grid.ColumnDefinitions>
                                                <ColumnDefinition Width="*" />
                                                <ColumnDefinition Width="Auto" />
                                            </Grid.ColumnDefinitions>

                                            <ContentPresenter
                                                x:Name="ContentSite"
                                                Grid.Column="0"
                                                ContentSource="Header"
                                                HorizontalAlignment="Center"
                                                Margin="5,2,5,2"
                                                RecognizesAccessKey="True"
                                                VerticalAlignment="Center" />

                                            <Button
                                                x:Name="CloseButton"
                                                Grid.Column="1"
                                                Command="{Binding CloseTabCommand, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}"
                                                CommandParameter="{Binding}"
                                                Margin="5,0,5,0"
                                                Style="{StaticResource CloseButtonStyle}" />
                                        </Grid>
                                    </Border>
                                </Grid>
                                <ControlTemplate.Triggers>
                                    <Trigger Property="IsSelected" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="{DynamicResource {x:Static SystemColors.WindowBrushKey}}" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="DarkGray" />
                                    </Trigger>
                                    <Trigger Property="IsSelected" Value="False">
                                        <Setter TargetName="Border" Property="Background" Value="LightGray" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="Gray" />
                                    </Trigger>
                                    <Trigger Property="IsMouseOver" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="#FFDDDDDD" />
                                    </Trigger>
                                </ControlTemplate.Triggers>
                            </ControlTemplate>
                        </Setter.Value>
                    </Setter>
                </DataTrigger>
                <DataTrigger
                    Binding="{Binding RelativeSource={RelativeSource Self}, Path=TabStripPlacement, Converter={StaticResource ResourceKey=DockOrientationConverter}}"
                    Value="Vertical">
                    <Setter
                        Property="Width"
                        Value="{Binding TabHeight, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" />
                    <Setter Property="Template">
                        <Setter.Value>
                            <ControlTemplate TargetType="TabItem">
                                <Grid>
                                    <Border
                                        x:Name="Border"
                                        Background="{TemplateBinding Background}"
                                        BorderBrush="{TemplateBinding BorderBrush}"
                                        BorderThickness="{TemplateBinding BorderThickness}"
                                        CornerRadius="2"
                                        Margin="2,2,2,0">
                                        <Grid Name="_btnGrid">
                                            <Grid.RowDefinitions>
                                                <RowDefinition Height="Auto" />
                                                <RowDefinition Height="*" />
                                            </Grid.RowDefinitions>

                                            <ContentPresenter
                                                x:Name="ContentSite"
                                                Grid.Row="{Binding 
                                                    RelativeSource={RelativeSource TemplatedParent}, 
                                                    Path=TabStripPlacement, 
                                                    Converter={StaticResource ResourceKey=GridRowAdjustment}, 
                                                    ConverterParameter=content}"
                                                ContentSource="Header"
                                                HorizontalAlignment="Center"
                                                Margin="5,2,5,2"
                                                RecognizesAccessKey="True"
                                                VerticalAlignment="Center" />

                                            <Button
                                                x:Name="CloseButton"
                                                Grid.Row="{Binding 
                                                    RelativeSource={RelativeSource TemplatedParent}, 
                                                    Path=TabStripPlacement, 
                                                    Converter={StaticResource ResourceKey=GridRowAdjustment}, 
                                                    ConverterParameter=button}"
                                                Command="{Binding CloseTabCommand, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}"
                                                CommandParameter="{Binding}"
                                                Margin="5,0,5,0"
                                                Style="{StaticResource CloseButtonStyle}" />
                                        </Grid>
                                    </Border>
                                </Grid>
                                <ControlTemplate.Triggers>
                                    <Trigger Property="IsSelected" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="{DynamicResource {x:Static SystemColors.WindowBrushKey}}" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="DarkGray" />
                                    </Trigger>
                                    <Trigger Property="IsSelected" Value="False">
                                        <Setter TargetName="Border" Property="Background" Value="LightGray" />
                                        <Setter TargetName="Border" Property="BorderBrush" Value="Gray" />
                                    </Trigger>
                                    <Trigger Property="IsMouseOver" Value="True">
                                        <Setter TargetName="Border" Property="Background" Value="#FFDDDDDD" />
                                    </Trigger>
                                </ControlTemplate.Triggers>
                            </ControlTemplate>
                        </Setter.Value>
                    </Setter>
                </DataTrigger>
            </Style.Triggers>
            <Setter
                Property="Width"
                Value="{Binding TabWidth, RelativeSource={RelativeSource AncestorType={x:Type local:DockingGroup}}}" />
        </Style>
    </TabControl.ItemContainerStyle>

</TabControl>
```

```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WpfDockingManager
{
    public partial class DragTabControl : TabControl
    {
        public DragTabControl()
        {
            InitializeComponent();
        }

        private void LeftScrollButton_Click(object sender, RoutedEventArgs e)
        {
            TabScrollViewer.ScrollToHorizontalOffset(TabScrollViewer.HorizontalOffset - 50);
        }

        private void RightScrollButton_Click(object sender, RoutedEventArgs e)
        {
            TabScrollViewer.ScrollToHorizontalOffset(TabScrollViewer.HorizontalOffset + 50);
        }

        private void TopScrollButton_Click(object sender, RoutedEventArgs e)
        {
            TabScrollViewer.ScrollToVerticalOffset(TabScrollViewer.VerticalOffset - 50);
        }

        private void BottomScrollButton_Click(object sender, RoutedEventArgs e)
        {
            TabScrollViewer.ScrollToVerticalOffset(TabScrollViewer.VerticalOffset + 50);
        }

        private void TabScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Your existing logic
        }

        private void OnPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Your existing logic
        }

        private void OnPreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Your existing logic
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            // Your existing logic
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            // Your existing logic
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            // Your existing logic
        }

        private void OnCloseButtonEvent(object sender, RoutedEventArgs e)
        {
            // Your existing logic
        }
    }
}
```

Key changes and explanations:

1.  **Template Modification**:
    *   The `TabControl` template is updated to include a `Grid` named `MainGrid` that hosts the `ScrollViewer` and scroll buttons.
    *   `LeftScrollButton`, `RightScrollButton`, `TopScrollButton`, and `BottomScrollButton` are defined within the `MainGrid`.
    *   The initial visibility of `TopScrollButton` and `BottomScrollButton` is set to `Collapsed`.
2.  **Triggers**:
    *   Triggers are added to the `ControlTemplate.Triggers` section to handle different `TabStripPlacement` values.
    *   When `TabStripPlacement` is `Left` or `Right`:
        *   The `Grid.ColumnDefinitions` and `Grid.RowDefinitions` of `MainGrid` are adjusted to support vertical layout.
        *   The `Grid.Row` and `Grid.Column` properties of `TabScrollViewer` are set to position it correctly.
        *   The visibility of `LeftScrollButton` and `RightScrollButton` is set to `Collapsed`.
        *   The visibility of `TopScrollButton` and `BottomScrollButton` is set to `Visible`.
    *   When `TabStripPlacement` is `Top` or `Bottom`:
        *   The visibility of `TopScrollButton` and `BottomScrollButton` is set to `Collapsed`.
        *   The visibility of `LeftScrollButton` and `RightScrollButton` is set to `Visible`.
3.  **Code-Behind**:
    *   The code-behind remains the same as in the previous example.

With these changes, the scroll buttons will dynamically adjust their layout and visibility based on the `TabStripPlacement` property of the `TabControl`. When the `TabStripPlacement` is `Left` or `Right`, the top and bottom buttons will be visible and functional, and when it's `Top` or `Bottom`, the left and right buttons will be visible and functional.
