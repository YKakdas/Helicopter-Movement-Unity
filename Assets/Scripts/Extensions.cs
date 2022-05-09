public static class Extensions {
	public static float SignedAngle(this float val) {
		return (val > 180) ? val - 360 : val;
	}
}
