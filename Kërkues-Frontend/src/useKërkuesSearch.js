import { useState, useEffect } from "react";
import { useHistory } from "react-router-dom";
import Axios from "axios";

const useKërkuesSearch = (term) => {
  const [data_, setData] = useState(null);
  const history = useHistory(); 

  useEffect(() => {	
    const fetchData = async () => {
	  if (term == null) {
	    term = (history.location.pathname).replace("/search=", "");
	  }
      Axios.post("https://localhost:7290/api/Search", {"search": term})
        .then(response => response.data)
		.then((result) => {setData(result);});
    };

    fetchData()
      .then((res) => {
        console.log(res);
      })
      .catch((error) => {
        console.log(error);
      });
  }, [term]);
  
  return data_;
};

export default useKërkuesSearch;
