import { useState, useEffect } from "react";
import Axios from "axios";

const useKërkuesSearch = (term) => {
  const [data_, setData] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
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
