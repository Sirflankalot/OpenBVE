using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibRender {
	public partial class Renderer {
		FileStream log_file;

		internal void Log(string message, Settings.Verbosity verbosity) {
			if (settings.log_file_location == null || log_file == null || verbosity > settings.vebosity) {
				return;
			}

			StringBuilder log_string = new StringBuilder();
			log_string.Append((int) verbosity);
			log_string.Append(':');
			log_string.Append(DateTime.Now.ToUniversalTime().ToString("HH:mm:ss.fff"));
			log_string.Append(':');
			log_string.Append("0"); // TODO: Frame Count
			log_string.Append(": ");
			log_string.Append(message);

			using (StreamWriter sw = new StreamWriter(log_file)) {
				sw.Write(log_string.ToString());
				sw.Flush();
			}
		}
	}
}
