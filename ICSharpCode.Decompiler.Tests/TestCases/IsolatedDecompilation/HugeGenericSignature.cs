// A nested type cannot be referenced through a derived class in metadata, so every "Y." step
// expands to X<Y, Y, Y, Y, Y>.Y with the previous type substituted five times.
// The signature of the field below is 5.9 MB large (#2064).
class X<A, B, C, D, E>
{
	class Y : X<Y, Y, Y, Y, Y>
	{
		Y.Y.Y.Y.Y.Y.Y.Y y;
	}
}
