# UnityXMLUI
The UI system in Unity 4.6 is fantastic, but there isn’t an efficient method for generating UI at runtime or support for templating designs. For simple static scenarios the WYSIWYG editor is sufficient, but I’ve found it difficult for non developers on my teams to create dynamic, data driven UI’s. I’ve also found myself repeating code between projects for simple tasks like rendering lists of items, swapping image backgrounds, updating data values, handling dialog boxes, etc. The goal of this project is to provide an extendable framework for addressing these issues, borrowing concepts from web frameworks like Angular and Jade. 

# Quick Start 
1. Clone the repo into a Unity3d project.
2. Create an empty GameObject and attach UITEST.cs
3. UIAssets will also be added to the GameObject. Add an entry to the Fonts field and select "Arial". Do the same for the Images field and select the "Background" sprite. 
4. Drag UI/Test/test.xml to the XML field on UITEST.cs
5. Hit play. 

#Design

##UI
The UI class provides a wrapper for managing an individual UI, with methods for searching contained elements and rendering templates. When instantiating a UI a IElementBuilder object must be supplied to establish the UI’s rendering rules.  

##RulesBuilderBase
This is the default implementation of IElementBuilder. It provides support for three types of elements, div, text, and button, along with a collection of tags which can be used with each of them. It provides a good base for building custom UI implementations providing utility methods for parsing Vector2’s from text, colors by hex or name, etc. 

##UIAssets
This global provides access to the fonts, sprites, and materials available for use in the UI. Currently it is very simple, but I would like to ultimately provide support for loading content from asset bundles and providing content variant support. 

##IUIBindable 
Implementing this interface is required for all objects UI elements will be bound to. Objects implementing this interface receive a reference to the UI element they bind on. To push an update to the bound UI element and all its children simply call the  simply call the Update method on the bound view. *An example of this can be found in the UITEST.cs script*.   

