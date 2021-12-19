import React from "react";
import { useStateValue } from "../StateProvider";
import useKërkuesSearch from "../useKërkuesSearch";
import "./SearchPage.css";
import Response from "../response";
import { Link } from "react-router-dom";
import Search from "./Search";
import ArrowDropDownIcon from "@material-ui/icons/ArrowDropDown";

function SearchPage() {
  const [{ term /* = "tesla" */ }, dispatch] = useStateValue();
  const { data } = useKërkuesSearch(term); // Live API CALL

  //Mock API CALL
  // const data = Response;

  console.log(data);

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
      </div>

      {term && (
        <div className="searchPage-results">
          <p className="searchPage-resultCount">
            About {data?.searchInformation.formattedTotalResults} results (
            {data?.searchInformation.formattedSearchTime} seconds) for {term}
          </p>

          {data?.items.map((item) => (
            <div className="searchPage-result">
              {/* eslint-disable-next-line react/jsx-no-target-blank */}
              <a href={item.link} target="_blank">
                {item.pagemap?.cse_image?.length > 0 &&
                  item.pagemap?.cse_image[0]?.src && (
                    <img
                      className="searchPage_resultImage"
                      src={
                        item.pagemap?.cse_image?.length > 0 &&
                        item.pagemap?.cse_image[0]?.src
                      }
                      alt=""
                    />
                  )}
                {item.displayLink} <ArrowDropDownIcon />
              </a>
              <a className="searchPage-resultTitle" href={item.link}>
                <h2>{item.title}</h2>
              </a>
              <p className="searchPage-resultSnippet">{item.snippet}</p>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default SearchPage;
