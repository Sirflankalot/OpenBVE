using OpenBveApi.Sounds;

namespace OpenBve {
	internal static partial class Sounds {
		
		/// <summary>Represents a sound buffer.</summary>
		internal class SoundBuffer : SoundHandle {
			// --- members ---
			/// <summary>The origin where the sound can be loaded from.</summary>
			internal SoundOrigin Origin;
			/// <summary>The default effective radius.</summary>
			internal double Radius;
			/// <summary>Whether the sound is loaded and the OpenAL sound name is valid.</summary>
			internal bool Loaded;
			/// <summary>The OpenAL sound name. Only valid if the sound is loaded.</summary>
			internal int OpenAlBufferName;
			/// <summary>The duration of the sound in seconds. Only valid if the sound is loaded.</summary>
			internal double Duration;
			/// <summary>Whether to ignore further attemps to load the sound after previous attempts have failed.</summary>
			internal bool Ignore;
			/// <summary>The volume factor to be applied when this sound is heard from inside the cab (External volume should be considered to be a nominal 1.0)</summary>
			internal double InternalVolumeFactor;
			/// <summary>The function script controlling this sound's pitch.</summary>
			internal FunctionScripts.FunctionScript PitchFunction;
			/// <summary>The function script controlling this sound's volume.</summary>
			internal FunctionScripts.FunctionScript VolumeFunction;

			
			// --- constructors ---
			/// <summary>Creates a new sound buffer</summary>
			/// <param name="path">The on-disk path to the sound to load</param>
			/// <param name="radius">The radius for this sound</param>
			internal SoundBuffer(string path, double radius) {
				this.Origin = new PathOrigin(path);
				this.Radius = radius;
				this.Loaded = false;
				this.OpenAlBufferName = 0;
				this.Duration = 0.0;
				this.InternalVolumeFactor = 0.5;
				this.Ignore = false;
				this.PitchFunction = null;
				this.VolumeFunction = null;
				
			}
			/// <summary>Creates a new sound buffer</summary>
			/// <param name="sound">The API handle to a loaded sound</param>
			/// <param name="radius">The radius for this sound</param>
			internal SoundBuffer(OpenBveApi.Sounds.Sound sound, double radius) {
				this.Origin = new RawOrigin(sound);
				this.Radius = radius;
				this.Loaded = false;
				this.OpenAlBufferName = 0;
				this.Duration = 0.0;
				this.InternalVolumeFactor = 0.5;
				this.Ignore = false;
				this.PitchFunction = null;
				this.VolumeFunction = null;
			}
			/// <summary>Creates a new uninitialized sound buffer</summary>
			internal SoundBuffer()
			{
				this.Origin = null;
				this.Radius = 0.0;
				this.Loaded = false;
				this.OpenAlBufferName = 0;
				this.Duration = 0.0;
				this.InternalVolumeFactor = 0.5;
				this.Ignore = false;
				this.PitchFunction = null;
				this.VolumeFunction = null;
			}

			/// <summary>Creates a clone of the specified sound buffer</summary>
			/// <param name="b">The buffer to clone</param>
			/// <returns>The new buffer</returns>
			internal SoundBuffer Clone(SoundBuffer b)
			{
				return new SoundBuffer
				{
					Origin = b.Origin,
					Radius = b.Radius,
					Loaded = false,
					OpenAlBufferName = 0,
					Duration = b.Duration,
					InternalVolumeFactor = b.InternalVolumeFactor,
					Ignore = false,
					PitchFunction = b.PitchFunction,
					VolumeFunction = b.VolumeFunction
				};
			}
		}
		
	}
}