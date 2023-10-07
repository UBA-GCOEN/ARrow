import userModel from "../models/userModel.js";

/**
 * Route: /profile
 * Desc: get profile details
 */
export const profile = async (req, res) => {
  res.send("Display profile page");
};

/**
 * Route: /profile/update
 * Desc: request to update profile
 */
export const updateProfile = async (req, res) => {
  /**
   * getting remaing fields
   * from requests.
   */
  console.log(req.body);
  const email = req.email;
  const newData = req.body;

  console.log("check1");
  //update User in database
  const result = await userModel.updateOne({ email: email }, { $set: newData });

  console.log("check2");
 
  const newResult = await userModel.findOne({ email });

  console.log("check3");
  console.log(newResult);
  if (result) {
    res.json({
      success: true,
      msg: "user " + newResult.role + " updated successfully",
    });
  }
};
