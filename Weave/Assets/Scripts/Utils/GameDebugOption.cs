using System.ComponentModel;
using UnityEngine;

public partial class SROptions
{
	private int _sequenceIndex = 0;
	// Options will be grouped by category

	[Category("Utilities")]
	public int SequenceIndex
	{
		get { return _sequenceIndex; }
		set { _sequenceIndex = value; }
	}

	[Category("Utilities")]
	public void JumpSequence()
	{
		GameManager.Instance.JumpSequence(_sequenceIndex);
	}


}