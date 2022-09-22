using System.Linq;

public interface IAssignable
{
	TaskPoint[] TaskPoints { get; set; }
	protected int RatsInPlace => TaskPoints.Where(p => p.rat != null && p.rat.ArrivedAtTask()).Count();

	public void AssignRat(Rat rat);
}
