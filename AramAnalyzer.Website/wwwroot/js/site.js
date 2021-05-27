// Save form info to local storage - to prevent spam (1 request/2 min)
$(document).ready(function () {
	var storage = window.localStorage;

	// Form submit event.
	$("#Form").submit(function (evt) {
		let name = storage.getItem("summonerName");
		let region = storage.getItem("region");
		let date = storage.getItem("date");

		// Check for spam
		if (name == $("#NameBox").val() && region == $("#RegionBox").val() && (new Date().getTime() < parseInt(date) + 120000)) {
			evt.preventDefault();
		}
		else {
			// Save form info for next check
			storage.setItem("summonerName", $("#NameBox").val());
			storage.setItem("region", $("#RegionBox").val());
			let newDate = new Date();
			storage.setItem("date", newDate.getTime());
		}
	});
});