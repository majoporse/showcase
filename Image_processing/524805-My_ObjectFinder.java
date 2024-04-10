package PB130;

import ij.IJ;
import ij.ImagePlus;
import ij.measure.ResultsTable;
import ij.plugin.filter.Analyzer;
import ij.plugin.filter.PlugInFilter;
import ij.process.ImageProcessor;
import ij.process.ByteProcessor;
import ij.process.ColorProcessor;
import ij.process.ShortProcessor;
import ij.process.FloatProcessor;
import java.util.Queue;
import java.util.ArrayDeque;
import java.util.ArrayList;
import java.util.Collections;

class Pixel {
	/** Pixel coordinates. */
	int x, y;

	/** Constructor. */
	Pixel(int x, int y) {
		this.x = x;
		this.y = y;
	}
}

class BoundingBox {
	/** The coordinates of the bounding box origin (i.e., its top-left corner). */
	Pixel origin;
	/** The width of the bounding box. */
	int width;
	/** The height of the bounding box. */
	int height;

	/** Constructor. */
	BoundingBox(Pixel origin) {
		this.origin = origin;
		width = 1;
		height = 1;
	}

	/**
	 * Update the bounding box parameters based on the coordinates of a newly added
	 * pixel.
	 */
	void update(Pixel pixel) {
		if (pixel.x < origin.x) {
			width += origin.x - pixel.x;
			origin.x = pixel.x;
		} else {
			width = Math.max(width, pixel.x - origin.x + 1);
		}

		if (pixel.y < origin.y) {
			height += origin.x - pixel.x;
			origin.y = pixel.y;
		} else {
			height = Math.max(height, pixel.y - origin.y + 1);
		}
	}
}

class Region {
	/** The bounding box of the region. */
	BoundingBox bbox;
	/** The region label. */
	int label;
	/** The region size. */
	int size;
	/** The average grayscale intensity within the region. */
	double avgI;
	/** X-coordinate of the region centroid. */
	double cX;
	/** Y-coordinate of the region centroid. */
	double cY;
	/** The region eccentricity. */
	double ecc;
	/** The region orientation. */
	double orient;

	/** Constructor. */
	Region(Pixel origin, int label) {
		this.bbox = new BoundingBox(origin);
		this.label = label;
		this.size = 0;
		this.avgI = 0.0;
		this.cX = 0.0;
		this.cY = 0.0;
		this.ecc = -1.0;
		this.orient = 0.0;
	}
}

/**
 * This plugin segments and classifies objects according to their shape in a
 * given Candy Crush board.
 */
public class My_ObjectFinder implements PlugInFilter {
	String title = null;

	public int setup(String arg, ImagePlus im) {
		if (im != null) {
			// store the short image title for future use
			title = im.getShortTitle();

			// this plugin accepts RGB images only
			return DOES_RGB + NO_CHANGES;
		} else {
			// no image is open
			IJ.noImage();
			return DONE;
		}
	}

	public void run(ImageProcessor ip) {
		// extract the blue channel from the input RGB image
		ByteProcessor bip = ((ColorProcessor) ip).getChannel(3, null);

		// the following two lines can be commented out, they are used for initial
		// visualization purposes only
		// ImagePlus bImg = new ImagePlus("Blue channel of " + title, bip);
		// bImg.show();

		// segment individual objects in 'bip' (objects are 8-connected components
		// without holes)
		// classify the segmented objects into three classes: square, elongated tilted,
		// and other (roughly circular) objects
		// describe and explain in detail the classification criteria followed
		// remember that it is forbidden to exploit any color or intensity information
		// for classification purposes
		// WRITE YOUR CODE HERE

		// create an output RGB image as a copy of the input RGB image
		// ImageProcessor out = ip.duplicate();
		ImageProcessor out = bip.duplicate();

		// overlay 'out' by thick inner contours of the segmented and classified
		// objects,
		// representing each group of objects by a different color
		// WRITE YOUR CODE HERE

		// ResultsTable table = Analyzer.getResultsTable();
		// table.showRowNumbers(false);
		
		// for (int i = 0; i < rList.size(); ++i) {
		// 	Region reg = rList.get(i);
		// 	table.incrementCounter();
		// 	table.addValue("ID", reg.label);
		// 	table.addValue("BBox Origin X", reg.bbox.origin.x);
		// 	table.addValue("BBox Origin Y", reg.bbox.origin.y);
		// 	table.addValue("BBox Width", reg.bbox.width);
		// 	table.addValue("BBox Height", reg.bbox.height);
		// 	table.addValue("Size", reg.size);
		// 	table.addValue("Average Intensity", reg.avgI);
		// 	table.addValue("Centroid X", reg.cX);
		// 	table.addValue("Centroid Y", reg.cY);
		// 	table.addValue("Eccentricity", reg.ecc);
		// 	table.addValue("Orientation", reg.orient);
		// 	table.addResults();
		// }
		// table.updateResults();


		medianFilter(out, 2);
		gradient(out);
		gradientThresholding(out);
		binaryEroDilWithDiskSE(out, 2, 255);
		// contrast(out);
		// unimodialThresholding(out);
		holeFilling(out);
		binaryEroDilWithDiskSE(out, 2, 0);
		binaryOpeningWithDiskSE(out, 5);
		ArrayList<Region> rList = regions(out);
		measure(out, rList);

		classifyEcc(out, rList);
		out = markClasiffied(out, ip);

		// IJ.showMessage("NOT IMPLEMENTED!");
		// NO CHANGES NEEDED AFTER THIS LINE

		// display the output RGB image
		ImagePlus outImg = new ImagePlus("My object classification of " + title, out);
		outImg.show();
	}

	public void gradientThresholding(ImageProcessor ip) {
		// create the output image as a copy of the input one
		// ImageProcessor out = ip.duplicate();

		// threshold
		int thresh = 0;

		int w = ip.getWidth();
		int h = ip.getHeight();

		ImageProcessor grad = new FloatProcessor(w, h);
		float dx;
		float dy;
		float g;
		float sum = 0;

		for (int y = 1; y < h - 1; ++y) {
			for (int x = 1; x < w - 1; ++x) {

				dy = (ip.getf(x, y + 1) - ip.getf(x, y - 1)) / 2f;
				dx = (ip.getf(x + 1, y) - ip.getf(x - 1, y)) / 2f;
				g = (float) Math.sqrt(dx * dx + dy * dy);
				grad.setf(x, y, g);
				sum += g;

			}
		}

		float t = 0;
		for (int i = 0; i < grad.getPixelCount(); ++i) {
			t += ip.get(i) * grad.getf(i) / sum;
		}

		thresh = (int) t;

		for (int i = 0; i < ip.getPixelCount(); ++i) {
			if (ip.get(i) < thresh) {
				ip.set(i, 0);
			} else {
				ip.set(i, 255);
			}
		}
	}

	public void contrast(ImageProcessor ip) 
	{
		// WRITE YOUR CODE HERE
		int max = ip.getPixelCount();
		for (int i = 0; i < max; ++i){
			int p = (int) ((float) ip.get(i) * 1.5 + 0.5);
			if ( p > 255) {
				p = 255;
			}
			ip.set(i, p);
		}
	}

	private ImageProcessor markClasiffied(ImageProcessor ip, ImageProcessor cp){
		ImageProcessor ipSmall = ip.duplicate();
		// binaryEroDilWithDiskSE(ip, 1, 0);
		binaryEroDilWithDiskSE(ipSmall, 5, 0);
		ImageProcessor res = cp.duplicate();
		for (int i = 0; i < ip.getPixelCount(); ++i){
			int cur = ip.get(i) - ipSmall.get(i);
			if (cur == 75){
				res.set(i, 1230000);
			} else if (cur == 150){
				res.set(i, 5500000);
			} else if (cur == 225){
				res.set(i, 12300000);
			}
		}
		return res;

	}

	private void holeFilling(ImageProcessor ip) {
		// int BACKGROUND_LABEL = 0; // alias for the background label of a binary image
		// int TMP_LABEL = 128; // alias for the temporary label used during flood
		// filling

		Queue<Pixel> q = new ArrayDeque<Pixel>();

		int w = ip.getWidth();
		int h = ip.getHeight();

		for (int i = 0; i < w; ++i) {
			q.add(new Pixel(i, 0));
			q.add(new Pixel(i, h - 1));
		}

		for (int i = 0; i < h; ++i) {
			q.add(new Pixel(0, i));
			q.add(new Pixel(w - 1, i));
		}

		int count = 0;

		while (!q.isEmpty()) {

			Pixel p = q.remove();
			int x = p.x;
			int y = p.y;

			if (x >= 0 && x < w && y >= 0 && y < h && ip.get(x, y) == 0) {
				++count;
				ip.set(p.x, p.y, 128);
				q.add(new Pixel(x + 1, y));
				q.add(new Pixel(x - 1, y));
				q.add(new Pixel(x, y + 1));
				q.add(new Pixel(x, y - 1));
			}
		}

		for (int y = 0; y < h; ++y) {
			for (int x = 0; x < w; ++x) {
				if (ip.get(x, y) == 128) {
					ip.set(x, y, 0);
				} else {
					ip.set(x, y, 255);
				}
			}
		}
	}

	private void gradient(ImageProcessor ip) {

		int w = ip.getWidth();
		int h = ip.getHeight();

		// allocate the output image
		ImageProcessor dup = ip.duplicate();

		for (int y = 0; y < h; ++y) {
			for (int x = 0; x < w; ++x) {
				
				if (x == 0 || y == 0 || x == w - 1 || y == h - 1) {
					ip.set(x, y, 0);
					continue;
				}
				float dy = (dup.getf(x, y + 1) - dup.getf(x, y - 1)) / 2f;
				float dx = (dup.getf(x + 1, y) - dup.getf(x - 1, y)) / 2f;
				ip.set(x, y, (int) Math.sqrt(dx * dx + dy * dy));
			}

		}
	}

	private void measure(ImageProcessor lip, ArrayList<Region> rList) {
		int h = lip.getHeight();
		int w = lip.getWidth();

		for (int y = 0; y < h; ++y) {
			for (int x = 0; x < w; ++x) {
				if (lip.get(x, y) == 0) {
					continue;
				}

				Region reg = rList.get(lip.get(x, y) - 1);
				++reg.size;
				reg.cX += x;
				reg.cY += y;
			}
		}

		for (Region region : rList) {
			region.cX /= region.size;
			region.cY /= region.size;
			double u11 = centralMoment(lip, region, 1, 1);
			double u20 = centralMoment(lip, region, 2, 0);
			double u02 = centralMoment(lip, region, 0, 2);
			region.orient = Math.toDegrees(Math.atan2(2 * u11, u20 - u02) / 2);
			region.ecc = (u20 + u02 + Math.sqrt((u20 - u02) * (u20 - u02) + 4 * u11 * u11)) /
					(u20 + u02 - Math.sqrt((u20 - u02) * (u20 - u02) + 4 * u11 * u11));
		}
	}

	private ArrayList<Region> regions(ImageProcessor tip) {
		int UNLABELED = 254;
		// size of the input image
		int w = tip.getWidth();
		int h = tip.getHeight();


		// create a labeled image initialized with zeros
		// ImageProcessor lip = new ShortProcessor(w, h);

		// mark all foreground pixels as UNLABELED
		int num = w * h;
		for (int i = 0; i < num; ++i) {
			if (tip.get(i) == 255) {
				tip.set(i, UNLABELED);
			}
		}

		// a list used for storing regions
		ArrayList<Region> rList = new ArrayList<Region>();

		// region label
		int label = 1;

		// go through the labeled image 'lip' and label individual 4-connected
		// foreground regions using a breadth-first flood filling routine
		for (int y = 0; y < h; ++y) {
			for (int x = 0; x < w; ++x) {
				// check if the current pixel is UNLABELED
				if (tip.get(x, y) == UNLABELED) {
					// IJ.showMessage("NOT IMPLEMENTED!");
					rList.add(floodFill(tip, x, y, label));
					++label;
				}
			}
		}

		return rList;
	}

	private Region floodFill(ImageProcessor lip, int x, int y, int label) {

		int UNLABELED = 254;
		// the region for the given seed and label
		Region reg = new Region(new Pixel(x, y), label);

		// size of the input image
		int w = lip.getWidth();
		int h = lip.getHeight();

		// create an empty queue
		Queue<Pixel> q = new ArrayDeque<Pixel>();

		// add the seed pixel to the tail of the queue
		q.add(new Pixel(x, y));

		while (!q.isEmpty()) {
			// remove a pixel from the head of the queue
			Pixel n = q.remove();

			// check if the pixel 'n' is within the image domain and is UNLABELED
			if (n.x >= 0 && n.x < w && n.y >= 0 && n.y < h && lip.get(n.x, n.y) == UNLABELED) {
				reg.bbox.update(n);
				lip.set(n.x, n.y, label);
				q.add(new Pixel(n.x - 1, n.y));
				q.add(new Pixel(n.x + 1, n.y));
				q.add(new Pixel(n.x, n.y - 1));
				q.add(new Pixel(n.x, n.y + 1));
			}
		}

		return reg;
	}

	private double centralMoment(ImageProcessor lip, Region reg, int p, int q) {
		int a = reg.bbox.origin.x;
		int b = reg.bbox.origin.y;

		int h = b + reg.bbox.height;
		int w = a + reg.bbox.width;

		double cm = 0;
		int size = 0;

		for (int y = b; y < h; ++y) {
			for (int x = a; x < w; ++x) {

				if (lip.get(x, y) != reg.label) {
					continue;
				}

				cm += Math.pow((x - reg.cX), p) * Math.pow((y - reg.cY), q);
			}
		}
		return cm;
	}

	private void classifyEcc(ImageProcessor cip, ArrayList<Region> rList) {
		// WRITE YOUR CODE HERE
		for (Region region : rList) {

			// SQUARE
			if (region.ecc > 1.15 && region.ecc < 1.35) {
				regionFill(cip, region, 75);
			// ELONGATED / TILTED
			} else if (region.ecc > 3 && region.ecc < 4) { // region.orient > 40 && region.orient < 50 && nefunguje
				regionFill(cip, region, 150);
			} else {
				regionFill(cip, region, 225);
			}
		}
	}

	private void regionFill(ImageProcessor cip, Region region, int label) {
		int x = region.bbox.origin.x;
		int y = region.bbox.origin.y;
		while (cip.get(x, y) != region.label) {
			++x;
		}
		regionFloodFill(cip, region, x, y, label);
	}

	private void regionFloodFill(ImageProcessor cip, Region region, int x, int y, int label) {

		int w = cip.getWidth();
		int h = cip.getHeight();

		Queue<Pixel> q = new ArrayDeque<Pixel>();

		q.add(new Pixel(x, y));

		while (!q.isEmpty()) {
			Pixel n = q.remove();

			if (region.label != label && n.x >= 0 && n.x < w && n.y >= 0 && n.y < h
					&& cip.get(n.x, n.y) == region.label) {
				cip.set(n.x, n.y, label);
				q.add(new Pixel(n.x - 1, n.y));
				q.add(new Pixel(n.x + 1, n.y));
				q.add(new Pixel(n.x, n.y - 1));
				q.add(new Pixel(n.x, n.y + 1));
			}
		}
	}

	private void medianFilter(ImageProcessor ip, int radius) {
		ImageProcessor tmp = ip.duplicate();

		// initialize a list for storing values in the square neighborhood
		ArrayList<Integer> neighbors = new ArrayList<Integer>();
		int w = tmp.getWidth();
		int h = tmp.getHeight();
		int result;

		for (int y = 0; y < h; ++y) {
			for (int x = 0; x < w; ++x) {

				for (int my = -radius; my <= radius; ++my) {
					for (int mx = -radius; mx <= radius; ++mx) {

						if (x + mx < 0 || x + mx >= w || y + my < 0 || y + my >= h) {
							continue;
						}
						neighbors.add(tmp.get(x + mx, y + my));
					}
				}

				result = getMedian(neighbors);
				ip.set(x, y, result);

				neighbors.clear();
			}
		}
	}

	private int getMedian(ArrayList<Integer> a) {
		// sort the list 'a'
		Collections.sort(a);

		// the length of the list 'a'
		int len = a.size();

		// return the median value of the sorted list 'a'
		// to get the i-th value in the list, call a.get(i)
		// WRITE YOUR CODE HERE
		int res;

		if (len % 2 == 0) {
			res = (int) (Math.round((a.get(len / 2) + a.get((len / 2) - 1)) / 2.0));
		} else {
			res = a.get((len / 2));
		}
		return res;
	}

	private void binaryErosionWithDiskSE(ImageProcessor ip, int radius)
	{
		binaryEroDilWithDiskSE(ip, radius, 0);
	}
	
	/** Apply the dilation operator with a disk structuring element of given radius to a given binary image. To deal with situations when
	  * the structuring element hits a coordinate outside the image domain, assume that the value at such a coordinate is set to 0. */
	private void binaryDilationWithDiskSE(ImageProcessor ip, int radius)
	{
		binaryEroDilWithDiskSE(ip, radius, 255);
	}

	private void binaryOpeningWithDiskSE(ImageProcessor ip, int radius)
	{
		// return the result of this operator in the image 'ip'
		// you can use the existing functions 'binaryErosionWithDiskSE' and 'binaryDilationWithDiskSE', even though they are not memory efficient
		// do not forget to complete the function 'binaryEroDilWithDiskSE'
		// WRITE YOUR CODE HERE
		binaryErosionWithDiskSE(ip, radius);
		binaryDilationWithDiskSE(ip, radius);
		//IJ.showMessage("NOT IMPLEMENTED!");	
	}

	/** Apply the closing operator with a disk structuring element of given radius to a given binary image. */
	private void binaryClosingWithDiskSE(ImageProcessor ip, int radius)
	{
		// return the result of this operator in the image 'ip'
		// you can use the existing functions 'binaryErosionWithDiskSE' and 'binaryDilationWithDiskSE', even though they are not memory efficient
		// do not forget to complete the function 'binaryEroDilWithDiskSE'
		// WRITE YOUR CODE HERE
		binaryDilationWithDiskSE(ip, radius);
		binaryErosionWithDiskSE(ip, radius);
		// IJ.showMessage("NOT IMPLEMENTED!");
	}

	/** Apply the erosion (critical_value equals to 0) or dilation (critical_value equals to 255) operator with a disk structuring element of given radius to a given binary image. To deal with situations when 
	  * the structuring element hits a coordinate outside the image domain, assume that the value at such a coordinate is set to 255 - critical_value. */
	private void binaryEroDilWithDiskSE(ImageProcessor ip, int radius, int critical_value)
	{
		// return the result of this operator in the image 'ip'
		// do not forget to duplicate the image 'ip' before computation
		// it is forbidden to use continue, break, or alike commands
		// WRITE YOUR CODE HERE

		int w = ip.getWidth();
		int h = ip.getHeight();
		int cur;
		ImageProcessor lol = ip.duplicate();

		for(int y = 0; y < h; ++y){
			for(int x = 0; x < w; ++x){

				boolean stop = false;

				for (int yy = -radius; yy <= radius && !stop; ++yy){
					for (int xx = -radius; xx <= radius && !stop; ++xx){

						if ((xx * xx) + (yy * yy) > radius * radius){
							continue;
						}
						if (x + xx < 0 || x + xx >= w || y + yy < 0 || y + yy >= h){
							continue;
						}
						cur = lol.get(x + xx, y + yy);
						if (cur == critical_value){
							stop = true;
						}
					}
				}

				if (stop){
					ip.set(x, y, critical_value);
				}

			}
		}
	}

}