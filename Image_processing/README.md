# Object Finder Plugin for IMAGEJ for Candy Crush Images

This plugin, implemented in `MyObjectFinder.java`, is designed to automatically and uniformly identify objects in the blue channel of attached RGB images of Candy Crush (*.tif format). It categorizes the objects into three groups based on their shapes: square objects, elongated rotated objects, and other (almost circular) objects. The classification criteria are thoroughly described and justified through comments in the code. Notably, during object classification, the use of color or intensity of objects is strictly prohibited.

Upon execution, the plugin opens a new window displaying an RGB image containing the visual information from the input image overlaid with internal boundaries of the identified objects. These objects are considered as 8-connected components without holes. The internal boundaries of objects from the same group are drawn in the same color.

The loading of input images is left to the user. Sample visual outputs are provided in the attached images (`output_candy_crush_*.tif`).
