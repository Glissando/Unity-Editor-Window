using System.Collections;

public class Quest {
	public string name;
	public string description;

	public int current = 0;
	public int count;

	public int money;
	public int exp;

	public bool returnToQuestGiver;
	public bool dirty;

	public Quest() {
		money = 0;
		exp = 0;
		returnToQuestGiver = false;
	}

	public Quest(string name) {
		this.name = name;
	}

	public Quest(Quest q) {
		this.name = q.name;
		this.description = q.description;
		this.current = q.current;
		this.count = q.count;
		this.money = q.money;
		this.exp = q.exp;
	}
	
	public override string ToString() {
		return(name + ": " + description);
	}
}
