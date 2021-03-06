﻿using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.ComponentModel;

namespace SkiaSharp
{
	public class SKTextBlob : SKObject
	{
		[Preserve]
		internal SKTextBlob (IntPtr x, bool owns)
			: base (x, owns)
		{
		}

		protected override void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero && OwnsHandle) {
				SkiaApi.sk_textblob_unref (Handle);
			}

			base.Dispose (disposing);
		}

		public SKRect Bounds {
			get {
				SkiaApi.sk_textblob_get_bounds (Handle, out var bounds);
				return bounds;
			}
		}

		public uint UniqueId => SkiaApi.sk_textblob_get_unique_id (Handle);
	}

	public class SKTextBlobBuilder : SKObject
	{
		[Preserve]
		internal SKTextBlobBuilder (IntPtr x, bool owns)
			: base (x, owns)
		{
		}

		public SKTextBlobBuilder ()
			: this (SkiaApi.sk_textblob_builder_new (), true)
		{
		}

		protected override void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero && OwnsHandle) {
				SkiaApi.sk_textblob_builder_delete (Handle);
			}

			base.Dispose (disposing);
		}

		// AddRun

		public void AddRun (SKPaint font, float x, float y, ushort[] glyphs) =>
			AddRunInternal (font, x, y, glyphs, null, null, null, false);

		public void AddRun (SKPaint font, float x, float y, ushort[] glyphs, SKRect bounds) =>
			AddRunInternal (font, x, y, glyphs, null, null, bounds, false);

		public void AddRun (SKPaint font, float x, float y, ushort[] glyphs, string text, uint[] clusters)
		{
			var utf8Text = StringUtilities.GetEncodedText (text, SKEncoding.Utf8);
			AddRunInternal (font, x, y, glyphs, utf8Text, clusters, null, true);
		}

		public void AddRun (SKPaint font, float x, float y, ushort[] glyphs, string text, uint[] clusters, SKRect bounds)
		{
			var utf8Text = StringUtilities.GetEncodedText (text, SKEncoding.Utf8);
			AddRunInternal (font, x, y, glyphs, utf8Text, clusters, bounds, true);
		}

		public void AddRun (SKPaint font, float x, float y, ushort[] glyphs, byte[] utf8Text, uint[] clusters) =>
			AddRunInternal (font, x, y, glyphs, utf8Text, clusters, null, true);

		public void AddRun (SKPaint font, float x, float y, ushort[] glyphs, byte[] utf8Text, uint[] clusters, SKRect bounds) =>
			AddRunInternal (font, x, y, glyphs, utf8Text, clusters, bounds, true);

		private void AddRunInternal (SKPaint font, float x, float y, ushort[] glyphs, byte[] utf8Text, uint[] clusters, SKRect? bounds, bool hasText)
		{
			if (font == null)
				throw new ArgumentNullException (nameof (font));
			if (glyphs == null)
				throw new ArgumentNullException (nameof (glyphs));

			if (hasText) {
				if (utf8Text == null)
					throw new ArgumentNullException (nameof (utf8Text));
				if (clusters == null)
					throw new ArgumentNullException (nameof (clusters));
				if (glyphs.Length != clusters.Length)
					throw new ArgumentException ("The number of glyphs and clusters must be the same.");
			}

			var run = AllocateRun (font, glyphs.Length, x, y, hasText ? utf8Text.Length : 0, bounds);
			run.SetGlyphs (glyphs);

			if (hasText) {
				run.SetText (utf8Text);
				run.SetClusters (clusters);
			}
		}

		// AddHorizontalRun

		public void AddHorizontalRun (SKPaint font, float y, ushort[] glyphs, float[] positions) =>
			AddHorizontalRunInternal (font, y, glyphs, positions, null, null, null, false);

		public void AddHorizontalRun (SKPaint font, float y, ushort[] glyphs, float[] positions, SKRect bounds) =>
			AddHorizontalRunInternal (font, y, glyphs, positions, null, null, bounds, false);

		public void AddHorizontalRun (SKPaint font, float y, ushort[] glyphs, float[] positions, string text, uint[] clusters)
		{
			var utf8Text = StringUtilities.GetEncodedText (text, SKEncoding.Utf8);
			AddHorizontalRunInternal (font, y, glyphs, positions, utf8Text, clusters, null, true);
		}

		public void AddHorizontalRun (SKPaint font, float y, ushort[] glyphs, float[] positions, string text, uint[] clusters, SKRect bounds)
		{
			var utf8Text = StringUtilities.GetEncodedText (text, SKEncoding.Utf8);
			AddHorizontalRunInternal (font, y, glyphs, positions, utf8Text, clusters, bounds, true);
		}

		public void AddHorizontalRun (SKPaint font, float y, ushort[] glyphs, float[] positions, byte[] utf8Text, uint[] clusters) =>
			AddHorizontalRunInternal (font, y, glyphs, positions, utf8Text, clusters, null, true);

		public void AddHorizontalRun (SKPaint font, float y, ushort[] glyphs, float[] positions, byte[] utf8Text, uint[] clusters, SKRect bounds) =>
			AddHorizontalRunInternal (font, y, glyphs, positions, utf8Text, clusters, bounds, true);

		private void AddHorizontalRunInternal (SKPaint font, float y, ushort[] glyphs, float[] positions, byte[] utf8Text, uint[] clusters, SKRect? bounds, bool hasText)
		{
			if (font == null)
				throw new ArgumentNullException (nameof (font));
			if (glyphs == null)
				throw new ArgumentNullException (nameof (glyphs));
			if (positions == null)
				throw new ArgumentNullException (nameof (positions));
			if (glyphs.Length != positions.Length)
				throw new ArgumentException ("The number of glyphs and positions must be the same.");

			if (hasText) {
				if (utf8Text == null)
					throw new ArgumentNullException (nameof (utf8Text));
				if (clusters == null)
					throw new ArgumentNullException (nameof (clusters));
				if (glyphs.Length != clusters.Length)
					throw new ArgumentException ("The number of glyphs and clusters must be the same.");
			}

			var run = AllocateRunHorizontal (font, glyphs.Length, y, hasText ? utf8Text.Length : 0, bounds);
			run.SetGlyphs (glyphs);
			run.SetPositions (positions);

			if (hasText) {
				run.SetText (utf8Text);
				run.SetClusters (clusters);
			}
		}

		// AddPositionedRun

		public void AddPositionedRun (SKPaint font, ushort[] glyphs, SKPoint[] positions) =>
			AddPositionedRunInternal (font, glyphs, positions, null, null, null, false);

		public void AddPositionedRun (SKPaint font, ushort[] glyphs, SKPoint[] positions, SKRect bounds) =>
			AddPositionedRunInternal (font, glyphs, positions, null, null, bounds, false);

		public void AddPositionedRun (SKPaint font, ushort[] glyphs, SKPoint[] positions, string text, uint[] clusters)
		{
			var utf8Text = StringUtilities.GetEncodedText (text, SKEncoding.Utf8);
			AddPositionedRunInternal (font, glyphs, positions, utf8Text, clusters, null, true);
		}

		public void AddPositionedRun (SKPaint font, ushort[] glyphs, SKPoint[] positions, string text, uint[] clusters, SKRect bounds)
		{
			var utf8Text = StringUtilities.GetEncodedText (text, SKEncoding.Utf8);
			AddPositionedRunInternal (font, glyphs, positions, utf8Text, clusters, bounds, true);
		}

		public void AddPositionedRun (SKPaint font, ushort[] glyphs, SKPoint[] positions, byte[] utf8Text, uint[] clusters) =>
			AddPositionedRunInternal (font, glyphs, positions, utf8Text, clusters, null, true);

		public void AddPositionedRun (SKPaint font, ushort[] glyphs, SKPoint[] positions, byte[] utf8Text, uint[] clusters, SKRect bounds) =>
			AddPositionedRunInternal (font, glyphs, positions, utf8Text, clusters, bounds, true);

		private void AddPositionedRunInternal (SKPaint font, ushort[] glyphs, SKPoint[] positions, byte[] utf8Text, uint[] clusters, SKRect? bounds, bool hasText)
		{
			if (font == null)
				throw new ArgumentNullException (nameof (font));
			if (glyphs == null)
				throw new ArgumentNullException (nameof (glyphs));
			if (positions == null)
				throw new ArgumentNullException (nameof (positions));
			if (glyphs.Length != positions.Length)
				throw new ArgumentException ("The number of glyphs and positions must be the same.");

			if (hasText) {
				if (utf8Text == null)
					throw new ArgumentNullException (nameof (utf8Text));
				if (clusters == null)
					throw new ArgumentNullException (nameof (clusters));
				if (glyphs.Length != clusters.Length)
					throw new ArgumentException ("The number of glyphs and clusters must be the same.");
			}

			var run = AllocateRunPositioned (font, glyphs.Length, hasText ? utf8Text.Length : 0, bounds);
			run.SetGlyphs (glyphs);
			run.SetPositions (positions);

			if (hasText) {
				run.SetText (utf8Text);
				run.SetClusters (clusters);
			}
		}

		//

		public SKTextBlob Build () =>
			GetObject<SKTextBlob> (SkiaApi.sk_textblob_builder_make (Handle));

		internal SKTextBlobBuilderRunBuffer AllocateRun (SKPaint font, int count, float x, float y, int textByteCount, SKRect? bounds)
		{
			if (font == null)
				throw new ArgumentNullException (nameof (font));

			using (var lang = new SKString ()) {
				unsafe {
					SKTextBlobBuilderRunBuffer runbuffer;
					if (bounds is SKRect b)
						SkiaApi.sk_textblob_builder_alloc_run_text (Handle, font.Handle, count, x, y, textByteCount, lang.Handle, &b, out runbuffer);
					else
						SkiaApi.sk_textblob_builder_alloc_run_text (Handle, font.Handle, count, x, y, textByteCount, lang.Handle, (SKRect*)IntPtr.Zero, out runbuffer);
					return runbuffer;
				}
			}
		}

		internal SKTextBlobBuilderRunBuffer AllocateRunHorizontal (SKPaint font, int count, float y, int textByteCount, SKRect? bounds)
		{
			if (font == null)
				throw new ArgumentNullException (nameof (font));

			using (var lang = new SKString ()) {
				unsafe {
					SKTextBlobBuilderRunBuffer runbuffer;
					if (bounds is SKRect b)
						SkiaApi.sk_textblob_builder_alloc_run_text_pos_h (Handle, font.Handle, count, y, textByteCount, lang.Handle, &b, out runbuffer);
					else
						SkiaApi.sk_textblob_builder_alloc_run_text_pos_h (Handle, font.Handle, count, y, textByteCount, lang.Handle, (SKRect*)IntPtr.Zero, out runbuffer);
					return runbuffer;
				}
			}
		}

		internal SKTextBlobBuilderRunBuffer AllocateRunPositioned (SKPaint font, int count, int textByteCount, SKRect? bounds)
		{
			if (font == null)
				throw new ArgumentNullException (nameof (font));

			using (var lang = new SKString ()) {
				unsafe {
					SKTextBlobBuilderRunBuffer runbuffer;
					if (bounds is SKRect b)
						SkiaApi.sk_textblob_builder_alloc_run_text_pos (Handle, font.Handle, count, textByteCount, lang.Handle, &b, out runbuffer);
					else
						SkiaApi.sk_textblob_builder_alloc_run_text_pos (Handle, font.Handle, count, textByteCount, lang.Handle, (SKRect*)IntPtr.Zero, out runbuffer);
					return runbuffer;
				}
			}
		}
	}

	[StructLayout (LayoutKind.Sequential)]
	internal struct SKTextBlobBuilderRunBuffer
	{
		private IntPtr glyphs;
		private IntPtr pos;
		private IntPtr utf8text;
		private IntPtr clusters;

		public IntPtr GlyphsBuffer => glyphs;

		public IntPtr PositionsBuffer => pos;

		public IntPtr Utf8textBuffer => utf8text;

		public IntPtr ClustersBuffer => clusters;

		public void SetGlyphs (IntPtr glyphs, int count)
		{
			unsafe {
				SkiaApi.sk_textblob_builder_runbuffer_set_glyphs (ref this, (ushort*)glyphs, count);
			}
		}

		public void SetGlyphs (ushort[] glyphs)
		{
			unsafe {
				fixed (ushort* g = glyphs) {
					SkiaApi.sk_textblob_builder_runbuffer_set_glyphs (ref this, g, glyphs.Length);
				}
			}
		}

		public void SetPositions (IntPtr pos, int count)
		{
			unsafe {
				SkiaApi.sk_textblob_builder_runbuffer_set_pos (ref this, (float*)pos, count);
			}
		}

		public void SetPositions (float[] pos)
		{
			unsafe {
				fixed (float* p = pos) {
					SkiaApi.sk_textblob_builder_runbuffer_set_pos (ref this, p, pos.Length);
				}
			}
		}

		public void SetPositions (SKPoint[] pos)
		{
			unsafe {
				fixed (SKPoint* p = pos) {
					SkiaApi.sk_textblob_builder_runbuffer_set_pos_points (ref this, p, pos.Length);
				}
			}
		}
		public void SetText (IntPtr utf8Text, int count)
		{
			unsafe {
				SkiaApi.sk_textblob_builder_runbuffer_set_utf8_text (ref this, (byte*)utf8Text, count);
			}
		}

		public void SetText (byte[] utf8Text)
		{
			unsafe {
				fixed (byte* t = utf8Text) {
					SkiaApi.sk_textblob_builder_runbuffer_set_utf8_text (ref this, t, utf8Text.Length);
				}
			}
		}

		public void SetText (string text)
		{
			var utf8Text = StringUtilities.GetEncodedText (text, SKEncoding.Utf8);
			SetText (utf8Text);
		}

		public void SetClusters (IntPtr clusters, int count)
		{
			unsafe {
				SkiaApi.sk_textblob_builder_runbuffer_set_clusters (ref this, (uint*)clusters, count);
			}
		}

		public void SetClusters (uint[] clusters)
		{
			unsafe {
				fixed (uint* c = clusters) {
					SkiaApi.sk_textblob_builder_runbuffer_set_clusters (ref this, c, clusters.Length);
				}
			}
		}
	}
}
