import React from "react";
import { useStateValue } from "../StateProvider";
import useKërkuesSearch from "../useKërkuesSearch";
import "./SearchPage.css";
import Response from "../response";
import { Link } from "react-router-dom";
import Search from "./Search";
import ArrowDropDownIcon from "@material-ui/icons/ArrowDropDown";
import { Button } from "@material-ui/core";
import { useHistory } from "react-router-dom";

function SearchPage() {
  const [{ term }, dispatch] = useStateValue();
  const history = useHistory();
  
  const data = useKërkuesSearch(term); // Live API CALL  
  //const data = Response; //Mock API CALL
  //console.log(data);
  
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
