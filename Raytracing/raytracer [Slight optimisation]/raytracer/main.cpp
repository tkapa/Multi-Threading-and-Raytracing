#include "CImg.h"
#include "kf/kf_vector.h"
#include "scene.h"
#include "renderable.h"
#include "kf/kf_ray.h"
#include "light.h"
#include "windows.h"
#include "luascript.h"
#include "kf/kf_time.h"
#include "kf/kf_algorithms.h"
#include "kf/kf_math.h"
#include <thread>
using namespace cimg_library;

//#define TIMING_PER_PIXEL
//#define SHOW_TIMING_PER_PIXEL
//#define SHOW_TIMING_PER_ROW
//#define SHOW_TIMING_PER_COL

// This controls if the rendering displays progressively. When timing the results, turn this off (otherwise updating the window repeatedly will affect the timing)
// progressiveCount is how many lines are rendered before updating the window.
bool progressiveDisplay = false;
int  progressiveCount   = 10;

// The resolution of the window and the output of the ray tracer. This can be overridden by the Lua startup script.
int windowWidth = 1024;
int windowHeight = 1024;
int halfWidth = windowWidth / 2;
int halfHeight = windowHeight / 2;
int qWidth = halfWidth / 2;
int qHeight = halfHeight / 2;

// The scene object.
Scene g_scene;

// Lua state object used to run the startup script.
lua_State *g_state;


int main(int argc, char **argv)
{
	srand(0);
	kf::Time timer;
	
	std::string startupScript = "scene.lua";
	if (argc > 1)
	{
		startupScript = argv[1];
	}

	initLua(startupScript);

	
	// The floating point image target that the scene is rendered into.
	CImg<float> image(windowWidth, windowHeight, 1, 3, 0);
	// The display object used to show the image.
	CImgDisplay main_disp(image, "Raytrace");
	main_disp.set_normalization(0);	// Normalisation 0 disables auto normalisation of the image (scales to make the darkest to brightest colour fit 0 to 1 range.

#ifdef TIMING_PER_PIXEL
	long long *timingData = new long long[windowWidth*windowHeight];
	long long minPixelTiming = 1000000000;
	long long maxPixelTiming = 0;
#endif

	// Record the starting time.
	long long startTime = timer.getTicks();

	//Top left
	std::thread t1([&image, &main_disp, &timer, &startTime] {
		// Primary loop through all screen pixels.
		for (int y = 0; y < halfHeight; y++)
		{
			for (int x = 0; x < halfWidth; x++)
			{
				#ifdef TIMING_PER_PIXEL
					long long startPixelTime = timer.getTicks();
				#endif
				// Retrieve the colour of the specified pixel. The math below converts pixel coordinates (0 to width) into camera coordinates (-1 to 1).
				kf::Colour output = g_scene.trace(float(x - halfWidth) / (halfWidth), -float(y - halfHeight) / (halfHeight)*((float(windowHeight) / windowWidth)));

				// Clamp the output colour to 0-1 range before conversion.
				// Convert from linear space to sRGB.
				output.toSRGB();
				output.saturate();
				// Write the colour to the image (scaling up by 255).
				*image.data(x, y, 0, 0) = output.r * 255;
				*image.data(x, y, 0, 1) = output.g * 255;
				*image.data(x, y, 0, 2) = output.b * 255;
			}

			// Perform progressive display if enabled.
			if (progressiveDisplay)
			{
				if (y % progressiveCount == 0)
				{
					main_disp.display(image);
				}
			}

			// Check for Escape key.
			if (main_disp.is_keyESC())
				return 0;
		}
	});

	//Top Right
	std::thread t2([&image, &main_disp, &timer, &startTime] {
		// Primary loop through all screen pixels.
		for (int y = 0; y < halfHeight; y++)
		{
			for (int x = halfWidth; x < windowWidth; x++)
			{
				#ifdef TIMING_PER_PIXEL
				long long startPixelTime = timer.getTicks();
				#endif
				// Retrieve the colour of the specified pixel. The math below converts pixel coordinates (0 to width) into camera coordinates (-1 to 1).
				kf::Colour output = g_scene.trace(float(x - halfWidth) / (halfWidth), -float(y - halfHeight) / (halfHeight)*((float(windowHeight) / windowWidth)));

				// Clamp the output colour to 0-1 range before conversion.
				// Convert from linear space to sRGB.
				output.toSRGB();
				output.saturate();
				// Write the colour to the image (scaling up by 255).
				*image.data(x, y, 0, 0) = output.r * 255;
				*image.data(x, y, 0, 1) = output.g * 255;
				*image.data(x, y, 0, 2) = output.b * 255;
			}

			// Perform progressive display if enabled.
			if (progressiveDisplay)
			{
				if (y % progressiveCount == 0)
				{
					main_disp.display(image);
				}
			}

			// Check for Escape key.
			if (main_disp.is_keyESC())
				return 0;
		}
	});

	//Bottom Left
	std::thread t3([&image, &main_disp, &timer, &startTime] {
		// Primary loop through all screen pixels.
		for (int y = halfHeight; y < halfHeight + qHeight; y++)
		{
			for (int x = 0; x < halfWidth; x++)
			{
#ifdef TIMING_PER_PIXEL
				long long startPixelTime = timer.getTicks();
#endif
				// Retrieve the colour of the specified pixel. The math below converts pixel coordinates (0 to width) into camera coordinates (-1 to 1).
				kf::Colour output = g_scene.trace(float(x - halfWidth) / (halfWidth), -float(y - halfHeight) / (halfHeight)*((float(windowHeight) / windowWidth)));

				// Clamp the output colour to 0-1 range before conversion.
				// Convert from linear space to sRGB.
				output.toSRGB();
				output.saturate();
				// Write the colour to the image (scaling up by 255).
				*image.data(x, y, 0, 0) = output.r * 255;
				*image.data(x, y, 0, 1) = output.g * 255;
				*image.data(x, y, 0, 2) = output.b * 255;
			}

			// Perform progressive display if enabled.
			if (progressiveDisplay)
			{
				if (y % progressiveCount == 0)
				{
					main_disp.display(image);
				}
			}

			// Check for Escape key.
			if (main_disp.is_keyESC())
				return 0;
		}
	});

	//Bottom Right
	std::thread t4([&image, &main_disp, &timer, &startTime] {
		// Primary loop through all screen pixels.
		for (int y = halfHeight; y < halfHeight + qHeight; y++)
		{
			for (int x = halfWidth; x < windowWidth; x++)
			{
				#ifdef TIMING_PER_PIXEL
				long long startPixelTime = timer.getTicks();
				#endif
				// Retrieve the colour of the specified pixel. The math below converts pixel coordinates (0 to width) into camera coordinates (-1 to 1).
				kf::Colour output = g_scene.trace(float(x - halfWidth) / (halfWidth), -float(y - halfHeight) / (halfHeight)*((float(windowHeight) / windowWidth)));

				// Clamp the output colour to 0-1 range before conversion.
				// Convert from linear space to sRGB.
				output.toSRGB();
				output.saturate();
				// Write the colour to the image (scaling up by 255).
				*image.data(x, y, 0, 0) = output.r * 255;
				*image.data(x, y, 0, 1) = output.g * 255;
				*image.data(x, y, 0, 2) = output.b * 255;
			}

			// Perform progressive display if enabled.
			if (progressiveDisplay)
			{
				if (y % progressiveCount == 0)
				{
					main_disp.display(image);
				}
			}

			// Check for Escape key.
			if (main_disp.is_keyESC())
				return 0;
		}
	});

	//Bottom Right
	std::thread t5([&image, &main_disp, &timer, &startTime] {
		// Primary loop through all screen pixels.
		for (int y = halfHeight + qHeight; y < windowHeight; y++)
		{
			for (int x = halfWidth; x < windowWidth; x++)
			{
				#ifdef TIMING_PER_PIXEL
				long long startPixelTime = timer.getTicks();
				#endif
				// Retrieve the colour of the specified pixel. The math below converts pixel coordinates (0 to width) into camera coordinates (-1 to 1).
				kf::Colour output = g_scene.trace(float(x - halfWidth) / (halfWidth), -float(y - halfHeight) / (halfHeight)*((float(windowHeight) / windowWidth)));

				// Clamp the output colour to 0-1 range before conversion.
				// Convert from linear space to sRGB.
				output.toSRGB();
				output.saturate();
				// Write the colour to the image (scaling up by 255).
				*image.data(x, y, 0, 0) = output.r * 255;
				*image.data(x, y, 0, 1) = output.g * 255;
				*image.data(x, y, 0, 2) = output.b * 255;
			}

			// Perform progressive display if enabled.
			if (progressiveDisplay)
			{
				if (y % progressiveCount == 0)
				{
					main_disp.display(image);
				}
			}

			// Check for Escape key.
			if (main_disp.is_keyESC())
				return 0;
		}
	});

	//Bottom Right
	std::thread t6([&image, &main_disp, &timer, &startTime] {
		// Primary loop through all screen pixels.
		for (int y = halfHeight + qHeight; y < windowHeight; y++)
		{
			for (int x = 0; x < halfWidth; x++)
			{
#ifdef TIMING_PER_PIXEL
				long long startPixelTime = timer.getTicks();
#endif
				// Retrieve the colour of the specified pixel. The math below converts pixel coordinates (0 to width) into camera coordinates (-1 to 1).
				kf::Colour output = g_scene.trace(float(x - halfWidth) / (halfWidth), -float(y - halfHeight) / (halfHeight)*((float(windowHeight) / windowWidth)));

				// Clamp the output colour to 0-1 range before conversion.
				// Convert from linear space to sRGB.
				output.toSRGB();
				output.saturate();
				// Write the colour to the image (scaling up by 255).
				*image.data(x, y, 0, 0) = output.r * 255;
				*image.data(x, y, 0, 1) = output.g * 255;
				*image.data(x, y, 0, 2) = output.b * 255;
			}

			// Perform progressive display if enabled.
			if (progressiveDisplay)
			{
				if (y % progressiveCount == 0)
				{
					main_disp.display(image);
				}
			}

			// Check for Escape key.
			if (main_disp.is_keyESC())
				return 0;
		}
	});

	t1.join();
	t2.join();
	t3.join();
	t4.join();
	t5.join();
	t6.join();

	// Record ending time.
	long long endTime = timer.getTicks();

	// Save the output to a bmp file.
	image.save_bmp("output.bmp");

#ifdef TIMING_PER_PIXEL
#ifdef SHOW_TIMING_PER_PIXEL
	for (int y = 0; y < windowHeight; y++)
	{
		for (int x = 0; x < windowWidth; x++)
		{
			float c = kf::remap<double>(minPixelTiming, maxPixelTiming, 0, 255, timingData[x + y*windowWidth]);
			*image.data(x, y, 0, 0) = c;
			*image.data(x, y, 0, 1) = c;
			*image.data(x, y, 0, 2) = c;
		}
	}
#endif
#ifdef SHOW_TIMING_PER_ROW
	long long *rows = new long long[windowHeight];
	long long minRowTiming = 1000000000;
	long long maxRowTiming = 0;
	for (int y = 0; y < windowHeight; y++)
	{
		rows[y] = 0;
		for (int x = 0; x < windowWidth; x++)
		{
			rows[y] += timingData[x + y*windowWidth];
		}
		if (rows[y] < minRowTiming)
			minRowTiming = rows[y];
		if (rows[y] > maxRowTiming)
			maxRowTiming = rows[y];
	}
	for (int y = 0; y < windowHeight; y++)
	{
		for (int x = 0; x < 50; x++)
		{
			float c = kf::remap<double>(minRowTiming, maxRowTiming, 0, 255, rows[y]);
			*image.data(x, y, 0, 0) = 0;
			*image.data(x, y, 0, 1) = c;
			*image.data(x, y, 0, 2) = 0;
		}
	}
#endif
#ifdef SHOW_TIMING_PER_ROW
	long long *cols = new long long[windowWidth];
	long long minColTiming = 1000000000;
	long long maxColTiming = 0;
	for (int x = 0; x < windowWidth; x++)
	{
		cols[x] = 0;
		for (int y = 0; y < windowHeight; y++)
		{
			cols[x] += timingData[x + y*windowWidth];
		}
		if (cols[x] < minColTiming)
			minColTiming = cols[x];
		if (cols[x] > maxColTiming)
			maxColTiming = cols[x];
	}
	for (int x = 0; x < windowWidth; x++)
	{
		for (int y = 0; y < 50; y++)
		{
			float c = kf::remap<double>(minColTiming, maxColTiming, 0, 255, cols[x]);
			*image.data(x, y, 0, 0) = 0;
			*image.data(x, y, 0, 1) = c;
			*image.data(x, y, 0, 2) = c;
		}
	}
#endif
	// Save the timing to a bmp file.
	image.save_bmp("timing.bmp");
#endif


	// Display elapsed time in the window title bar.
	main_disp.set_title("Render time: %fs", timer.ticksToSeconds(endTime - startTime));
	main_disp.display(image);


	// Keep refreshing the window until it is closed or escape is hit.
	while (!main_disp.is_closed())
	{
		if (main_disp.is_keyESC())
			return 0;
		main_disp.wait();
	}

	return 0;

}
