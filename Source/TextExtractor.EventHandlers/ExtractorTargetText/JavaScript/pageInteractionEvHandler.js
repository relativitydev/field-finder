//if kCura.EventHandler.Helper.PageMode.Edit
$(document).ready(function () {
	var isStopMarkerApplied = false;
	var isPlainText = false;
	var $markerType = $('[faartifactid=1043013]').parent().find('td select');
	var $startMarkerTextBox = $('[faartifactid=1040761]').closest('tr');
	var $stopMarkerTextBox = $('[faartifactid=1044124]').closest('tr');
	var $regExStartContainer = $('[faartifactid=1043027]').closest('tr');
	var $regExStopContainer = $('[faartifactid=1044125]').closest('tr');
	var $applyStopMarkerInput = $('[faartifactid=1047529]').closest('tr'); //.parent().find('input:last')
	var $caseSensitiveContainer = $('[faartifactid=1040772]').closest('tr');
	var $directionContainer = $('[faartifactid=1040769]').closest('tr');
	var $occurrenceContainer = $('[faartifactid=1040769]').closest('tr');
	var $charLenContainer = $('[faartifactid=1040769]').closest('tr');
	var $minExtractionsContainer = $('[faartifactid=1040769]').closest('tr');
	var $maxExtractionsContainer = $('[faartifactid=1040769]').closest('tr');
	var $delimiterContainer = $('[faartifactid=1040769]').closest('tr');
	var initialViewChanged = false;
		
	function hideInitialFields(){
		var radioValue = $applyStopMarkerInput.find('input[checked]').val();
			
		if(radioValue === 'True'){ //stop marker applied
			isStopMarkerApplied = true;
		}
			
		if($markerType.find('option[selected]').val() === plainText){
			isPlainText = true;
			showCommonFields();
			toggleContainersVisibility();
		} else if ( $markerType.find('option[selected]').val() === regEx){
			showCommonFields();
			toggleContainersVisibility();
		} else {
			$startMarkerTextBox.hide();
			$stopMarkerTextBox.hide();
			$regExStartContainer.hide(); 
			$regExStopContainer.hide();
			$caseSensitiveContainer.hide();
			$directionContainer.hide();
			$occurrenceContainer.hide();
			$charLenContainer.hide();
			$minExtractionsContainer.hide();
			$maxExtractionsContainer.hide();
			$delimiterContainer.hide();
		}
	}
		
	hideInitialFields();
	//markerType
	$markerType.change(function(){
		showCommonFields();
			
		if(this.value === '1043015'){//choice Plain
			isPlainText = true;
		} else if(this.value == '1043014'){ //choice RegEx
			isPlainText = false;
		}
			
		toggleContainersVisibility();
	});
		
	//Apply StopMarker
	$applyStopMarkerInput.find('input:nth-child(1)').change(function(){
		showCommonFields();
			
		var radioValue = $applyStopMarkerInput.find('input:last').val();
		if(radioValue === '0'){ //stop marker applied
			isStopMarkerApplied = true;
		} else {
			isStopMarkerApplied = false;
		}
			
		toggleContainersVisibility();
	});
		
	function toggleContainersVisibility(){
		if(isPlainText){
			$startMarkerTextBox.show();
			$caseSensitiveContainer.show();
			$regExStartContainer.hide();
			$regExStopContainer.hide();
				
			if(isStopMarkerApplied){
			    $stopMarkerTextBox.show();
			} else {
			    $stopMarkerTextBox.hide();			   
			}			
		} else {
		    $caseSensitiveContainer.hide();
		    $startMarkerTextBox.hide();
			$regExStartContainer.show();
			$stopMarkerTextBox.hide();
				
			if (isStopMarkerApplied) {
				$regExStopContainer.show();
			} else {
				$regExStopContainer.hide();
			}	
		}
			
		if(isStopMarkerApplied){
		    $directionContainer.hide();		    
		} else {
			$directionContainer.show();
		}
	}	
		
	function showCommonFields(){
		if(!initialViewChanged){			
			$occurrenceContainer.show();
			$charLenContainer.show();
			$minExtractionsContainer.show();
			$maxExtractionsContainer.show();
			$delimiterContainer.show();
		}
			
		initialViewChanged = true;
	}
});

//if kCura.EventHandler.Helper.PageMode.View
//if kCura.EventHandler.Helper.PageMode.Edit
$(document).ready(function () {
    var isStopMarkerApplied = false;
    var isPlainText = false;
    var $markerType = $('[faartifactid=1043013]').parent().find('td select');
    var $startMarkerTextBox = $('[faartifactid=1040761]').closest('tr');
    var $stopMarkerTextBox = $('[faartifactid=1044124]').closest('tr');
    var $regExStartContainer = $('[faartifactid=1043027]').closest('tr');
    var $regExStopContainer = $('[faartifactid=1044125]').closest('tr');
    var $applyStopMarkerInput = $('[faartifactid=1047529]').closest('tr'); //.parent().find('input:last')
    var $caseSensitiveContainer = $('[faartifactid=1040772]').closest('tr');
    var $directionContainer = $('[faartifactid=1040769]').closest('tr');
    var $occurrenceContainer = $('[faartifactid=1040769]').closest('tr');
    var $charLenContainer = $('[faartifactid=1040769]').closest('tr');
    var $minExtractionsContainer = $('[faartifactid=1040769]').closest('tr');
    var $maxExtractionsContainer = $('[faartifactid=1040769]').closest('tr');
    var $delimiterContainer = $('[faartifactid=1040769]').closest('tr');
    var initialViewChanged = false;

    function hideInitialFields() {
        var radioValue = $applyStopMarkerInput.find('input[checked]').val();

        if (radioValue === 'True') { //stop marker applied
            isStopMarkerApplied = true;
        }

        if ($markerType.find('option[selected]').val() === plainText) {
            isPlainText = true;
            showCommonFields();
            toggleContainersVisibility();
        } else if ($markerType.find('option[selected]').val() === regEx) {
            showCommonFields();
            toggleContainersVisibility();
        } else {
            $startMarkerTextBox.hide();
            $stopMarkerTextBox.hide();
            $regExStartContainer.hide();
            $regExStopContainer.hide();
            $caseSensitiveContainer.hide();
            $directionContainer.hide();
            $occurrenceContainer.hide();
            $charLenContainer.hide();
            $minExtractionsContainer.hide();
            $maxExtractionsContainer.hide();
            $delimiterContainer.hide();
        }
    }

    hideInitialFields();
    //markerType
    $markerType.change(function () {
        showCommonFields();

        if (this.value === '1043015') {//choice Plain
            isPlainText = true;
        } else if (this.value == '1043014') { //choice RegEx
            isPlainText = false;
        }

        toggleContainersVisibility();
    });

    //Apply StopMarker
    $applyStopMarkerInput.find('input:nth-child(1)').change(function () {
        showCommonFields();

        var radioValue = $applyStopMarkerInput.find('input:last').val();
        if (radioValue === '0') { //stop marker applied
            isStopMarkerApplied = true;
        } else {
            isStopMarkerApplied = false;
        }

        toggleContainersVisibility();
    });

    function toggleContainersVisibility() {
        if (isPlainText) {
            $startMarkerTextBox.show();
            $caseSensitiveContainer.show();
            $regExStartContainer.hide();
            $regExStopContainer.hide();

            if (isStopMarkerApplied) {
                $stopMarkerTextBox.show();
            } else {
                $stopMarkerTextBox.hide();
            }
        } else {
            $caseSensitiveContainer.hide();
            $startMarkerTextBox.hide();
            $regExStartContainer.show();
            $stopMarkerTextBox.hide();

            if (isStopMarkerApplied) {
                $regExStopContainer.show();
            } else {
                $regExStopContainer.hide();
            }
        }

        if (isStopMarkerApplied) {
            $directionContainer.hide();
        } else {
            $directionContainer.show();
        }
    }

    function showCommonFields() {
        if (!initialViewChanged) {
            $occurrenceContainer.show();
            $charLenContainer.show();
            $minExtractionsContainer.show();
            $maxExtractionsContainer.show();
            $delimiterContainer.show();
        }

        initialViewChanged = true;
    }
});
