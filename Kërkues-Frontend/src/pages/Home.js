import React from "react";
import "./Home.css";
import Search from "./Search";

function Home() {
  return (
    <div className="home">
      <div className="home-body">
        <img
          src="./logo.png"
          alt=""
        />
        <div className="home-inputContainer">
          <Search />
        </div>
      </div>
    </div>
  );
}

export default Home;
