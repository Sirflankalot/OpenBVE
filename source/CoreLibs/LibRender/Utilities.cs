using System;
using System.Collections.Generic;

namespace LibRender {
	internal static class Utilities {
		internal static void AssertValidIndicies<T>(List<T> list, int start, int end) {
			if (!(0 <= start && 0 <= end && end <= list.Count && (end == 0 ? start == end : start < end))) {
				throw new ArgumentException("Range invalid");
			}
		}
	}
}
