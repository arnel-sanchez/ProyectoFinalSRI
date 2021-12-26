import React, { useState } from "react";
import SearchIcon from "@material-ui/icons/Search";
import "./Search.css";
import { Button } from "@material-ui/core";
import { useHistory } from "react-router-dom";
import { useStateValue } from "../StateProvider";
import { actionTypes } from "../reducer";

function Search({ hideButtons = false }) {
  const [{ }, dispatch] = useStateValue();
  const [input, setInput] = useState("");
  const history = useHistory();

  const search = (e) => {
	if (input === '') { 
	  e.preventDefault();
	} 
	else  {
      e.preventDefault();
      console.log("You hit search:", input);
      dispatch({
        type: actionTypes.SET_SEARCH_TERM,
        term: input,
      });
      if (e.isTrusted) {
        history.push("/search=" + input);
	  }
	}
  };

  return (
    <form className="search">
      <div className="search-input">
        <SearchIcon className="search-inputIcon" />
        <input 
		  id='textfield'
		  value={input} 
		  onChange={(e) => setInput(e.target.value)} 
		/>
      </div>

      {!hideButtons ? (
        <div className="search-buttons">
          <Button 
			type="submit" 
			onClick={ search } 
			variant="outlined"
		  >
            Search Data
          </Button>
        </div>
      ) : (
        <div className="search-buttons">
          <Button
		    id='searchbutton' 
            className="search-buttonsHidden"
            type="submit"
            onClick={ search }
            variant="outlined"
          >
            Search Data
          </Button>
        </div>
      )}
    </form>
  );
}

export default Search;
