public interface IBurnable {

	void StartBurning();
	void StopBurning();
	bool IsBurning { get; }

}
