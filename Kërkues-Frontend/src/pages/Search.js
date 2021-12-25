import React, { useState } from "react";
import SearchIcon from "@material-ui/icons/Search";
import "./Search.css";
import { Button } from "@material-ui/core";
import { useHistory } from "react-router-dom";
import { useStateValue } from "../StateProvider";
import { actionTypes } from "../reducer";

function Search({ hideButtons = false }) {
  const [{ term }, dispatch] = useStateValue();
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
      /* do something input */

      history.push("/search=" + input);
	}
  };

  return (
    <form className="search">
      <div className="search-input">
        <SearchIcon className="search-inputIcon" />
        <input value={input} onChange={(e) => setInput(e.target.value)} />
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
