import React, { useEffect, useState } from "react";
import { useStateValue } from "../StateProvider";
import useKërkuesSearch from "../useKërkuesSearch";
import "./SearchPage.css";
import { Link } from "react-router-dom";
import Search from "./Search";
import { Button } from "@material-ui/core";
import { useHistory } from "react-router-dom";

function SearchPage() {  
  const [{ term }, dispatch] = useStateValue();
  const [ locationKeys, setLocationKeys ] = useState([])
  const history = useHistory(); 
  
  const update = () => {
	const newlocation = (history.location.pathname).replace("/search=", "")
	if (newlocation.includes("/")) {
	  return;
	}
	const input = document.getElementById('textfield')
	const lastValue = input.value;
	input.value = newlocation
	var event = new Event('input', { bubbles: true});
	const tracker = input._valueTracker;
	if (tracker) {
	  tracker.setValue(lastValue);
	}
	input.dispatchEvent(event);		  
	const button = document.getElementById('searchbutton')
	button.click()
  }
  
  useEffect(() => {
    if (performance.navigation.type === 1) {
      // Reload	  
    } else {
	  // Not reload	
    }
	update();
  }, [ document.getElementById('textfield') ]);
  
  useEffect(() => {
    return history.listen(location => {
      if (history.action === 'PUSH') {
        setLocationKeys([ location.key ])
      }
      else if (history.action === 'POP') {		
        if (locationKeys[1] === location.key) {
          setLocationKeys(([ _, ...keys ]) => keys)  
          // Forward event
        } 
		else {
          setLocationKeys((keys) => [ location.key, ...keys ])  
          // Back event  
        }
		update();
      }
    })
  }, [ locationKeys ])
  
  const data = useKërkuesSearch(term);
  const local = "http://127.0.0.1:8080/"
  const file = (item) => {return local + encodeURI(item.split("\\").at(-1))}

  return (
    <div className="search-page">
      <div className="searchPage-header">
        <Link to="/">
          <img
            className="searchPage-logo"
            src="./logo.png"
            alt=""
          />
        </Link>

        <div className="searchPage-headerBody">
          <Search hideButtons />
        </div>
				
		<div className="search-buttons">
          <Button 
			type="submit" 
			onClick={() => {history.push("")}} 
			variant="outlined" 
			style={{position: 'absolute', bottom:60, left:550,}}
		  >
            Go Home
          </Button>
        </div>
      </div>

      {term && (
        <div className="searchPage-results">
          <p className="searchPage-resultCount">
            About {data?.searchObjectResults.length} results (
            {data?.responseTime} seconds) for {term}
          </p>

          {data?.searchObjectResults.map((item) => (
            <div className="searchPage-result">
              {/* eslint-disable-next-line react/jsx-no-target-blank */}
              <a href={file(item.location)} target="blank" rel="noopener noreferrer">               
                {item.location} 
              </a>
              <a className="searchPage-resultTitle" href={file(item.location)}>
                <h2>{item.name}</h2>
              </a>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default SearchPage;
